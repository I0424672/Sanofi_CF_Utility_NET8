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
    public class PublicFunction
    {
        public static object Eval(object ThisObject)
        {
            
            try
            {
                ActionFunctionParams afp = (ActionFunctionParams)ThisObject;
                DataPoint dp_action = (DataPoint)afp.ActionDataPoint;

                // fetch the current subject from datapoint dp_action
                Subject current_subject = dp_action.Record.Subject;
                // fetch the current instance from datapoint dp_action
                Instance current_ins = dp_action.Record.DataPage.Instance;
                // fetch the current datapage from datapoint dp_action
                DataPage current_datapage = dp_action.Record.DataPage;
                // fetch the current logline from datapoint dp_action
                Record current_logline = dp_action.Record;

                

                // find the datapoint "TEST" on the same logline with dp_action
                DataPoint dp_TEST = dp_action.Record.DataPoints.FindByFieldOID("TEST");

                // find the datapoint "TEST1" from master log but on the same datapage with datapoint dp_action
                DataPoint dp_TEST1 = dp_action.Record.DataPage.MasterRecord.DataPoints.FindByFieldOID("TEST1");

                // find the datapage "TEST" within the same instance with dp_action
                DataPage dpg_TEST = dp_action.Record.DataPage.Instance.DataPages.FindByFormOID("TEST");

                // find the first datapage "TEST1" within the same instance with dp_action
                DataPage dpg_TEST1 = dp_action.Record.DataPage.Instance.DataPages.FindDataPageByFormOIDAndOrdinal("TEST1", 1);

                // find all instances for subject current_subject
                Instances inss_all = current_subject.Instances;

                // find the target instance "TEST" for the current subject current_subject
                Instance ins_TEST = current_subject.Instances.FindByFolderOID("TEST");

                // fetch all datapoints VISDAT for subject current_subject, among all folders and forms
                DataPoints dps_VISDATs = CustomFunction.FetchAllDataPointsForOIDPath("VISDAT", null, null, current_subject, false);

                // fetch all datapoints VISDAT for subject current_subject, on form "SV_01", among all folders
                DataPoints dps_VISDATs_SV_01 = CustomFunction.FetchAllDataPointsForOIDPath("VISDAT", "SV_01", null, current_subject, false);

                // fetch all datapoints VISDAT for subject current_subject, on form "SV_01", in folder "110" only
                DataPoints dps_VISDATs_SV_01_110 = CustomFunction.FetchAllDataPointsForOIDPath("VISDAT", "SV_01", "110", current_subject, false);



                //rename instance current_ins by adding "XX" as suffix
                string target_foldername_suffix = "XX";
                if (current_ins != null && current_ins.Active)
                    current_ins.SetInstanceName(target_foldername_suffix);

                //rename datapage current_datapage by adding "XX" as suffix
                string current_formname = current_datapage.Form.Name;
                string target_formname_suffix = "XX";
                if (current_datapage != null && current_datapage.Active)
                    current_datapage.Name = current_formname + " " + target_formname_suffix;

                //rename datapage current_datapage by complete new name new_formname
                string new_formname = "newname";
                if (current_datapage != null && current_datapage.Active)
                    current_datapage.Name = new_formname;

                

                //merge matrix "MTX" in subject current_subject
                int CRFVersionID = dp_action.Record.Subject.CRFVersionID;
                Matrix target_MTX = Matrix.FetchByOID("MTX", CRFVersionID);
                current_subject.MergeMatrix(target_MTX);

                //add target form with OID = "TEST" in folder current_ins
                int CRFVersionID1 = dp_action.Record.Subject.CRFVersionID;
                Form form_to_trigger = Form.FetchByOID("TEST", CRFVersionID1);
                current_ins.AddCRF(form_to_trigger, CRFVersionID);

                //add 5 loglines in current_datapage
                int current_loglinenumbder_currentdatapage = current_datapage.Records.Count;
                if (current_loglinenumbder_currentdatapage <= 5)
                {
                    for (int i = current_loglinenumbder_currentdatapage; i <= 5; i++)
                    current_datapage.AddLogRecord();
                }
                

                //open query with query text = "querytext" on datapoint dp_action, 
                CustomFunction.PerformQueryAction("querytext", 1, false, false, dp_action, true, afp.CheckID, afp.CheckHash);

                //close query with query text = "querytext" on datapoint dp_action
                CustomFunction.PerformQueryAction("querytext", 1, false, false, dp_action, false, afp.CheckID, afp.CheckHash);



                //make datapoint dp_action visible
                if (dp_action != null && dp_action.Active)
                    dp_action.IsVisible = true;

                //make datapoint dp_action invisible
                if (dp_action != null && dp_action.Active && dp_action.Data == string.Empty)
                    dp_action.IsVisible = false;



                //derive data "TARGET_VALUE" in datapoint "dp_action"
                if (dp_action != null && dp_action.Active && dp_action.Data != "TARGET_VALUE")
                    dp_action.Enter("TARGET_VALUE", string.Empty, 0);


                //set calendar for insatnce current_ins based on target date dt_target and its target date
                DateTime dt_target = (DateTime) dp_action.StandardValue();
                current_ins.SetTimeForward(dt_target, 0);
                
            }
            catch
            {
            }
            return null;

        }
    }
}