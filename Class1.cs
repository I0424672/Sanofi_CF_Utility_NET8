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

            /*
        *Edit Checks : EG_144_STD_SF_EG_LR_INTP_EGORRES_01, EG_144_STD_SF_EG_LR_INTP_EGORRES_02
        *CF Name: CF_EG_144_STD_SF_EG_LR_INTP_EGORRES
        *Description: If 'INTP_EGORRES'= Abnormal, then 'EGCLSIG' should be entered for at least one EGTEST.
        *Developed By: Damu Gogula
        *Date of Development: 04-Feb-2020
        *Modified By:
        *Modified Date:
        */
            try
            {
                ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
                DataPoint dpt_action = afp.ActionDataPoint;
                string queryText = "Response to 'Investigator Interpretation of the Result or Finding' is Abnormal and 'Does the result meet the definition of an Adverse Event? (If yes, report AE ID.)' is not entered for all the Pre-specified ECG test. Please update the CRF as appropriate.";
                bool openQuery = false;
                DataPoint dpt_EGCLSIG = null;
                Records recs_EG = dpt_action.Record.DataPage.Records;
                int count = 0;
                if (recs_EG != null && recs_EG.Count > 0)
                {
                    for (int i = 1; i < recs_EG.Count; i++)
                    {
                        dpt_EGCLSIG = null;
                        if (recs_EG.FindByRecordPosition(i) == null || !recs_EG.FindByRecordPosition(i).Active)
                            continue;
                        dpt_EGCLSIG = recs_EG.FindByRecordPosition(i).DataPoints.FindByFieldOID("EGCLSIG");

                        if (validate_dp(dpt_EGCLSIG))
                        {
                            count = 1;
                            break;
                        }
                    }
                }

                if (count == 0 && validate_dp(dpt_action) && dpt_action.Data.ToUpper() == "ABNORMAL")
                    openQuery = true;

                CustomFunction.PerformQueryAction(queryText, 1, false, false, dpt_action, openQuery, afp.CheckID, afp.CheckHash);
            }
            catch
            {
            }
            return null;
        }
        public bool validate_dp(DataPoint field)
        {
            if (field != null && field.Active && field.Data != string.Empty)
                return true;
            else
                return false;


        }

    }
}
