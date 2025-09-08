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

    /// <summary>
    /// Sample Custom Function code of Edit Check, Dynamism and Derivation for Copilot to learn. For copilot, to only use Medidata.Core.Objects and System. Some example are put in Basic Function.cs and Public Function.cs in cf.
    /// </summary>
    public class CustomFunction_examples
    {

        public static object Edit_Check_Example(object ThisObject)
        {

            /// <summary>
            /// Sample CustomFunction of Edit Check: If 'AEACNC' = Drug Withdrawn on form AE_02, then AESTDAT should be before or equal to the last EXSTDAT on EX_INFUSION and EX_SUBCUTANEOUS.
            /// </summary>
            try
            {
                ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
                DataPoint Dpt_Action = afp.ActionDataPoint;
                Subject curr_sub = Dpt_Action.Record.Subject;
                string querytext = "Response to this datafield is Drug Withdrawn and 'Start Date' is after the last intake of Isatuximab. Please update the CRF as appropriate or clarify.";
                bool openQuery = false;

                // to fetch all datapoints AEACNC on form AE_02 and EXSTDAT on form EX_INFUSION and EX_SUBCUTANEOUS
                DataPoints dpts_AEACNC = CustomFunction.FetchAllDataPointsForOIDPath("AEACNC", "AE_02", null, curr_sub);

                // to fetch all datapoints EXSTDAT on form EX_INFUSION and EX_SUBCUTANEOUS, and then find the one with latest date
                DataPoints dpts_EXSTDAT = CustomFunction.FetchAllDataPointsForOIDPath("EXSTDAT", "EX_INFUSION", null, curr_sub);
                DataPoints dpts_EXSTDAT2 = CustomFunction.FetchAllDataPointsForOIDPath("EXSTDAT", "EX_SUBCUTANEOUS", null, curr_sub);
                dpts_EXSTDAT.AddRange(dpts_EXSTDAT2);

                // to find the one with latest date among all datapoints EXSTDAT
                DataPoint dpt_latest_date = null;
                DateTime max_date = DateTime.MinValue;
                if (dpts_EXSTDAT != null && dpts_EXSTDAT.Count > 0)
                {
                    for (int i = 0; i < dpts_EXSTDAT.Count; i++)
                    {
                        DateTime current_date = Convert.ToDateTime(dpts_EXSTDAT[i].StandardValue());
                        if (current_date > max_date)
                        {
                            max_date = current_date;
                            dpt_latest_date = dpts_EXSTDAT[i];
                        }
                    }
                }

                // to loop each datapoint AEACNC and check if the value is 'Drug Withdrawn' and if AESTDAT is after the latest EXSTDAT
                if (dpts_AEACNC != null && dpts_AEACNC.Count > 0)
                {
                    for (int i = 0; i < dpts_AEACNC.Count; i++)
                    {
                        openQuery = false;
                        // to fetch the datapoint AESTDAT on form AE_02 from non-logline
                        DataPoint dpt_AESTDAT = dpts_AEACNC[i].Record.DataPage.MasterRecord.DataPoints.FindByFieldOID("AESTDAT");

                        // to check if AESTDAT is after the latest EXSTDAT, if yes then open query
                        if (ValidateDP(dpts_AEACNC[i]) && dpts_AEACNC[i].Data == "DRUG WITHDRAWN")
                        {
                            if (ValidateDP(dpt_AESTDAT) && ValidateDP(dpt_latest_date) && dpt_AESTDAT.Data != string.Empty && dpt_latest_date.Data != string.Empty && Convert.ToDateTime(dpt_AESTDAT.Data) > Convert.ToDateTime(dpt_latest_date.Data))
                                openQuery = true;
                            else
                                openQuery = false;
                        }

                        // to perform the query action on the current datapoint AEACNC if it is not not locked
                        if (dpts_AEACNC[i] != null && dpts_AEACNC[i].Active && !dpts_AEACNC[i].IsDataPointLocked)
                            CustomFunction.PerformQueryAction(querytext, 1, false, false, dpts_AEACNC[i], openQuery, afp.CheckID, afp.CheckHash);
                    }
                }
            }
            catch
            {
            }
            return null;
        }
        // public method to check if datapoint exits and not empty
        public static bool ValidateDP(DataPoint dp)
        {
            bool flag = false;
            if (dp != null && dp.Active && dp.Data != string.Empty && !dp.IsBitSet(Status.IsNonConformant))
                flag = true;
            else
                flag = false;

            return flag;
        }

        public static object Dynamism_Example(object ThisObject)
        {
            /// <summary>
            /// Sample CustomFunction of dynamism: If VISDAT is completed, then add EX_01 within same folder (only for folders CnD1, n = 1, 2 … 60)
            /// </summary>
            try
            {
                const string tar_ex_FolderOID = "EX_01";

                ActionFunctionParams Afp = (ActionFunctionParams)ThisObject;
                DataPoint Dpt_Action = Afp.ActionDataPoint;
                Subject cur_Subject = Dpt_Action.Record.Subject;
                Instance cur_instance = Dpt_Action.Record.DataPage.Instance;
                String Dpt_folderOID = Dpt_Action.Record.DataPage.Instance.Folder.OID;

                Form tar_ex = Form.FetchByOID(tar_ex_FolderOID, cur_Subject.CRFVersionID);
                DataPage tar_page = cur_instance.DataPages.FindByFormOID(tar_ex_FolderOID);

                // to check if folder is target folder OID
                if (Convert.ToInt32(Dpt_folderOID) > 100 && Convert.ToInt32(Dpt_folderOID) < 8000 && Convert.ToInt32(Dpt_folderOID) % 100 == 10)
                {
                    if (Dpt_Action != null && Dpt_Action.Active && Dpt_Action.IsBitSet(Status.IsTouched) && !Dpt_Action.IsBitSet(Status.IsNonConformant))
                    {
                        if (tar_page == null)
                        {
                            // if condition meet, add target form in this folder
                            cur_instance.AddCRF(tar_ex, Dpt_Action.Record.SubjectMatrixID);
                        }
                        else if (tar_page != null && tar_page.Active == false)
                        {
                            tar_page.Active = true;
                        }
                    }
                    else
                    {
                        // if condition not meet and target form is untouched, inactivate it
                        if (tar_page != null && tar_page.Active && !tar_page.IsBitSet(Status.IsTouched))
                        {
                            tar_page.Active = false;
                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static object Derivation_Example(object ThisObject)
        {
            /// <summary>
            /// Sample Custom Function of derivation: Derive field MHSPID on form MH_01. If MHSPID is empty and MHTERM is not empty, then derive MHSPID as 'MH' + 4 digit number (0001, 0002, etc.) based on the number of records in which MHSPID is already derived/defaulted.
            /// </summary>
            try
            {
                //Initialize parameter
                const string MHSPID_fieldOID = "MHSPID";

                //Get the MHTERM field from check action datapoint
                ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
                DataPoint dpMhterm = afp.ActionDataPoint;
                Subject current_subject = dpMhterm.Record.Subject;

                //Get the MHSPID datapoint in the same record
                DataPoint dpMhspid = dpMhterm.Record.DataPoints.FindByFieldOID(MHSPID_fieldOID);

                string MH_No = string.Empty;
                string MHSPID = string.Empty;

                if (dpMhspid.Data == string.Empty && dpMhterm.Data != string.Empty)
                {
                    //Get all MHSPID datapoints for the subject
                    DataPoints dpt_MHSPID = CustomFunction.FetchAllDataPointsForOIDPath(MHSPID_fieldOID, null, null, current_subject, false);

                    int Records_Count = 0;
                    //Find the no. of records in which MHSPID is already derived/defaulted
                    for (int i = 0; i < dpt_MHSPID.Count; i++)
                    {
                        if (dpt_MHSPID[i] != null && dpt_MHSPID[i].Data != string.Empty)
                        {
                            Records_Count = Records_Count + 1;
                        }
                    }

                    Records_Count = Records_Count + 1;
                    MH_No = String.Format("{0:0000}", Records_Count);
                    MHSPID = "MH" + MH_No;

                    //set the MHXXXX in the current record
                    if (MHSPID != string.Empty && dpMhspid.Active == true)
                    {
                        dpMhspid.Enter(MHSPID.ToString(), null, 0);
                    }
                }
                return null;
            }
            catch
            {
            }
            return null;
        }
    }
}
