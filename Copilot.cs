using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Medidata.Core.Objects;
// using Medidata.EmailService;
using Medidata.Core.Common;
using Medidata.Core.Common.Utilities;
using Medidata.Utilities;
using Medidata.Utilities.Interfaces;
using System.Configuration;
using System.IO;

namespace CustomFunctions
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.DirectoryServices.ActiveDirectory;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata.Ecma335;
    using System.Runtime;
    using System.Runtime.Intrinsics.Arm;
    using System.Timers;
    using System.Windows.Forms.VisualStyles;
    using Medidata.Core.Objects.Coding;
    #region System Class - Do Not Modify
    using Medidata.CustomFunctions.Debug;
    // using Mono.CSharp;
    //using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    /// <summary>
    /// Runs the CustomFunction Development Utility.
    /// </summary>
    public class SystemClass_C1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="_SystemClass"/> class.
        /// </summary>
        public SystemClass_C1()
        {
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            // System.Windows.Forms.Application.Run(new MainForm());
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
            // if (!CacheManager.IsThreadAbortException(exception)) System.Windows.Forms.MessageBox.Show(exception.ToString());
        }
    }
    #endregion

    /// <summary>
    /// Sample Custom Function code of Edit Check, Dynamism and Derivation for Copilot to learn. For copilot, to only use Medidata.Core.Objects and System. Some example are put in Basic Function.cs and Public Function.cs in cf.
    /// </summary>
    public class CustomFunction_c1
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
