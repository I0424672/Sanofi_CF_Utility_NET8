using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Medidata.Core.Objects;
using Medidata.Core.Common;
using Medidata.Core.Common.Utilities;
using Medidata.Utilities;
using Medidata.Utilities.Interfaces;
using System.Configuration;
using System.IO;


namespace CustomFunctions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms.VisualStyles;
    using Medidata.Core.Objects.Coding;
    using Medidata.CustomFunctions.Debug;
    // using Mono.CSharp;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
    public class BasicFunction
    {
        public static object Eval(object ThisObject)
        {
            try
            {
                ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
                DataPoint dp_action = (DataPoint)afp.ActionDataPoint;
                Subject current_subject = dp_action.Record.Subject;

                if (ValidateDatapoint(dp_action)) return true;

                DataPoints dpts_EXSTDAT = CustomFunction.FetchAllDataPointsForOIDPath("EXSTDAT", null, null, current_subject, false);
                DateTime max_date = Find_Max_Date(dpts_EXSTDAT);

                DataPoints dpts_EXENDAT = CustomFunction.FetchAllDataPointsForOIDPath("EXENDAT", null, null, current_subject, false);
                DateTime min_date = Find_Min_Date(dpts_EXSTDAT);

                DataPoint dp_AESTDAT = dp_action.Record.DataPoints.FindByFieldOID("AESTDAT");
                DataPoint dp_AEENDAT = dp_action.Record.DataPoints.FindByFieldOID("AEENDAT");
                bool result = Incomplete_Date_Compare(dp_AESTDAT, dp_AEENDAT, ">=");

            }
            catch
            {
            }
            return null;
        }
        /// <summary>
        /// Validates a data point.
        /// </summary>
        /// <param name="dp"> The data point to validate.
        /// <returns> True if the data point is active and not empty; otherwise, false.</returns>
        public static bool ValidateDatapoint(DataPoint dp)
        {
            if (dp != null && dp.Active && dp.Data != string.Empty && !dp.IsDataPointNonConformant)
                return true;
            return false;
        }

        /// <summary>
        /// To fetch the maximum date within the collection of datapoints dps.
        /// </summary>
        /// <param name="dps">The collection of datapoints to find the maximum date.</param>
        /// <returns>Return the maximum date (latest date).</returns>
        public static DateTime Find_Max_Date(DataPoints dps)
        {
            DateTime max_date = DateTime.MinValue;
            for (int i = 0; i < dps.Count; i++)
            {
                if (dps[i] != null && dps[i].Active && dps[i].Data != string.Empty)
                {
                    DateTime dp_date = (DateTime)dps[i].StandardValue();
                    if (dp_date > max_date)
                        max_date = dp_date;
                }
            }
            return max_date;
        }

        /// <summary>
        /// Finds the minimum date from the given collection of data points.
        /// </summary>
        /// <param name="dps">The collection of data points.</param>
        /// <returns>The minimum date found.</returns>
        public static DateTime Find_Min_Date(DataPoints dps)
        {
            DateTime min_date = DateTime.MaxValue;
            for (int i = 0; i < dps.Count; i++)
            {
                if (dps[i] != null && dps[i].Active && dps[i].Data != string.Empty)
                {
                    DateTime dp_date = (DateTime)dps[i].StandardValue();
                    if (dp_date < min_date)
                        min_date = dp_date;
                }
            }
            return min_date;
        }


        /// <summary>
        /// To compare two imcomplete dates between datapoints dp1 and dp2, based on condition.
        /// </summary>
        /// <param name="dp1">The first date.</param>
        /// <param name="dp2">The second date.</param>
        /// <param name="condition">The condition to check, including <, >, <=, >=, </param>
        /// <returns>The minimum date found.</returns>
        public static bool Incomplete_Date_Compare(DataPoint dp1, DataPoint dp2, string condition)
        {
            bool result = false;
            DateTime dt1 = DateTime.MinValue;
            DateTime dt2 = DateTime.MinValue;
            int yr1 = 0, yr2 = 0, mn1 = 0, mn2 = 0;
            if (dp1 != null && dp2 != null)
            {
                if (dp1.Data.Contains("UNK") || dp2.Data.Contains("UNK"))
                {
                    dt1 = Convert.ToDateTime(dp1.StandardValue());
                    yr1 = dt1.Year;
                    dt2 = Convert.ToDateTime(dp2.StandardValue());
                    yr2 = dt2.Year;
                    switch (condition)
                    {
                        case "<":
                            if (yr1 != 1800 && yr2 != 1800 && yr1 < yr2)
                                result = true;
                            break;
                        case ">":
                            if (yr1 != 1800 && yr2 != 1800 && yr1 > yr2)
                                result = true;
                            break;
                        case "<=":
                            if (yr1 != 1800 && yr2 != 1800 && yr1 <= yr2)
                                result = true;
                            break;
                        case ">=":
                            if (yr1 != 1800 && yr2 != 1800 && yr1 >= yr2)
                                result = true;
                            break;
                    }
                }
                else if (dp1.Data.StartsWith("UN") || dp2.Data.StartsWith("UN"))
                {
                    dt1 = Convert.ToDateTime(dp1.StandardValue());
                    yr1 = dt1.Year;
                    mn1 = dt1.Month;
                    dt2 = Convert.ToDateTime(dp2.StandardValue());
                    yr2 = dt2.Year;
                    mn2 = dt2.Month;
                    if (yr1 != 1800 && yr2 != 1800 && yr1 == yr2)
                    {
                        switch (condition)
                        {
                            case "<":
                                if (mn1 < mn2)
                                    result = true;
                                break;
                            case ">":
                                if (mn1 > mn2)
                                    result = true;
                                break;
                            case "<=":
                                if (mn1 <= mn2)
                                    result = true;
                                break;
                            case ">=":
                                if (mn1 >= mn2)
                                    result = true;
                                break;
                        }
                    }
                    else if (yr1 != 1800 && yr2 != 1800)
                    {
                        switch (condition)
                        {
                            case "<":
                                if (yr1 < yr2)
                                    result = true;
                                break;
                            case ">":
                                if (yr1 > yr2)
                                    result = true;
                                break;
                            case "<=":
                                if (yr1 <= yr2)
                                    result = true;
                                break;
                            case ">=":
                                if (yr1 >= yr2)
                                    result = true;
                                break;
                        }
                    }
                }
                else if (dp1.StandardValue() is DateTime && dp2.StandardValue() is DateTime)
                {
                    dt1 = (DateTime)dp1.StandardValue();
                    dt2 = (DateTime)dp2.StandardValue();
                    switch (condition)
                    {
                        case "<":
                            if ((dt1 < dt2) && dt1.Year != 1800 && dt2.Year != 1800)
                                result = true;
                            break;
                        case ">":
                            if ((dt1 > dt2) && dt1.Year != 1800 && dt2.Year != 1800)
                                result = true;
                            break;
                        case "<=":
                            if ((dt1 <= dt2) && dt1.Year != 1800 && dt2.Year != 1800)
                                result = true;
                            break;
                        case ">=":
                            if ((dt1 >= dt2) && dt1.Year != 1800 && dt2.Year != 1800)
                                result = true;
                            break;
                    }
                }
            }
            return result;
        }

    }
}