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
    using Medidata.Core.Objects.Coding;
    #region System Class - Do Not Modify
    using Medidata.CustomFunctions.Debug;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    /// <summary>
    /// Runs the CustomFunction Development Utility.
    /// </summary>
    public class _SystemClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="_SystemClass"/> class.
        /// </summary>
        public _SystemClass()
        {
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
        }
    }
    #endregion

    /// <summary>
    /// Sample CustomFunction.
    /// </summary>
    public class CustomFunction1
    {

        public object Eval(object ThisObject)
        {

            // add page in current instance (mutiple conditio)
            ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
            DataPoint Dpt_action = afp.ActionDataPoint;
            Subject current_subject = Dpt_action.Record.Subject;
            int CRFVersionID = current_subject.CRFVersionID;
            Instance cur_ins = Dpt_action.Record.DataPage.Instance;

            string Tri_str_1 = "custom value";
            string Tri_str_2 = "custom value";
            string addform_OID = "formOID";
            bool blnTrigForm = false;
            //Supposed oth_dps_1 is logline field, please replace with logline datapoints here
            DataPoints oth_dps_1 = CustomFunction.FetchAllDataPointsForOIDPath("ath_field_to_trigger_1", "ath_form_OID_1", "ath_folder_OID_1", current_subject);
            //Supposed oth_dp_2 is another standard field except Dpt_action, please repalce with another required standard datapoint here
            DataPoint oth_dp_2 = CustomFunction.FetchAllDataPointsForOIDPath("ath_field_to_trigger_2", "ath_form_OID_2", "ath_folder_OID_2", current_subject)[0];
            
            try
            {
                bool blnTrigForm_1 = CheckDataPoint(Dpt_action);
                bool blnTrigForm_2 = false;
                bool blnTrigForm_3 = CheckDataPoint(oth_dp_2) && oth_dp_2.Data == Tri_str_2;
                if (oth_dps_1 != null && oth_dps_1.Count > 0)
                {
                    for (int i = 0; i < oth_dps_1.Count; i++)
                    {
                        DataPoint oth_dp = oth_dps_1[i];
                        blnTrigForm_2 = (CheckDataPoint(oth_dp) && (oth_dp.Data == Tri_str_1));
                        if (blnTrigForm_2)
                        {
                            break;
                        }
                    }
                }
                blnTrigForm = blnTrigForm_1 && blnTrigForm_2 && blnTrigForm_3;
                if (blnTrigForm)
                {
                    AddForm(cur_ins, addform_OID, CRFVersionID);
                }
                else
                    InactiveForm(cur_ins, addform_OID);
            }
            catch
            {
            }
            return null;







        }

    }
}
