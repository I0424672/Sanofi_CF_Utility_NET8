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

            try
            {
                /*
                Developer: [Pavan Kumar Kanchi ICON]
                Date: [20 JUN 2023]
                Custom Function: [CF_EX_083_MF_EX_01_EXENDAT]
                Edit Checks: [EX_083_MF_EX_01_EXENDAT_01, EX_083_MF_EX_01_EXENDAT_02]
                Short Description: [If 'EXDSNTTK' is not checked, when EXSTDAT is entered in next visit (current folder OID+100 till 5910) then 'EXENDAT' should be entered.)
                *Applicable for all Follow up Visit]
                Protocol Requirement : []
                Modification History: [NA]
                */

                ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
                DataPoint dpt_action = afp.ActionDataPoint;
                Subject current_subject = dpt_action.Record.Subject;
                bool openQuery = false;
                string queryText = "Not taken' is not checked and 'End Date' is not entered when next visit Pomalidomide start date reported. Please update the CRF as appropriate.";


                Instance inst = dpt_action.Record.DataPage.Instance;
                string Fld_OID = inst.Folder.OID;

                if ((dpt_action.Field.OID == "EXENDAT" && inst.Folder.OID == "6010") || (dpt_action.Field.OID == "EXSTDAT" && inst.Folder.OID == "110")) return null;

                DataPoints dpts = new DataPoints();
                DataPoints dpts_Next = new DataPoints();

                if (dpt_action.Field.OID == "EXENDAT")
                {
                    dpts.Add(dpt_action);

                    if (Number.IsValidInteger(Fld_OID))

                        dpts_Next = CustomFunction.FetchAllDataPointsForOIDPath("EXSTDAT", "EX_01", (Convert.ToInt32(Fld_OID) + 100).ToString(), current_subject);
                }
                else
                {
                    dpts = CustomFunction.FetchAllDataPointsForOIDPath("EXENDAT", "EX_01", (Convert.ToInt32(Fld_OID) - 100).ToString(), current_subject);
                    dpts_Next = CustomFunction.FetchAllDataPointsForOIDPath("EXSTDAT", "EX_01", Fld_OID, current_subject);

                }


                if (dpts.Count > 0)

                {
                    bool Ex_st = false;

                    if (dpts_Next.Count > 0)
                    {
                        for (int i = 0; i < dpts_Next.Count; i++)
                        {
                            if (validate_dp(dpts_Next[i]))
                            {
                                Ex_st = true;
                                break;
                            }
                        }
                    }




                    for (int k = 0; k < dpts.Count; k++)
                    {
                        openQuery = false;
                        if (dpts[k] != null && dpts[k].Active)
                        {
                            if (dpts[k].Data == string.Empty && Ex_st)
                            {
                                DataPoint dpt_EXDSNTTK = dpts[k].Record.DataPage.MasterRecord.DataPoints.FindByFieldOID("EXDSNTTK");
                                if (dpt_EXDSNTTK != null && dpt_EXDSNTTK.Active && dpt_EXDSNTTK.Data == "0")
                                {
                                    openQuery = true;
                                }
                            }

                            if (!dpts[k].IsDataPointLocked)
                            {
                                // open a query with query text queryText on dpts[k]
                                CustomFunction.PerformQueryAction(queryText, 1, false, false, dpts[k], openQuery, afp.CheckID, afp.CheckHash);

                                // open a query with query text queryText on dp_action
                                CustomFunction.PerformQueryAction(queryText, 1, false, false, dpt_action, openQuery, afp.CheckID, afp.CheckHash);
                            }


                        }
                    }
                }
            }
            catch
            {
            }
            return null;
        }
        //Validate Dp
        public bool validate_dp(DataPoint Field)
        {
            if (Field != null && Field.Active && Field.Data != string.Empty && !Field.IsBitSet(Status.IsNonConformant))
                return true;
            else
                return false;




        }

    }
}
