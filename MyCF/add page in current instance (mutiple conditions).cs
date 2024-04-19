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


namespace CustomFunction1
{

    public class CustomFunction1
    {

        public object class1(object ThisObject)
        {
            /*
            * @CF Name: ADD_PAGE_IN_CURRENT_FOLDER_UNDER_MUTIPLE_CONDITIONS
            * @Dpt_action: please note the Dpt_action (check action) should be the required std field in the page (one of the triggers to add form)
            * @classification: Derivation
            * @function introduction: add requried page in current folder (where Dpt_action is) when mutiple conditions are satisfied
            * @note:Tri_str is the string value which required std field should be equal to; addform_OID means the form OID which will be added in designed place (cur_ins);
            *       oth_dps are datapoints used to store datapoints which should be used in another condition
            * @Created  for DEMO
            * @Author: Shuangshuang (2020/02/05)
            */

            // add page in current instance (mutiple conditions)
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
            DataPoints oth_dps_1 = CustomFunction.FetchAllDataPointsForOIDPath("ath_field_to_trigger_1", "ath_form_OID_1", "ath_folder_OID_1",current_subject);
            //Supposed oth_dp_2 is another standard field except Dpt_action, please repalce with another required standard datapoint here
            DataPoint oth_dp_2 = CustomFunction.FetchAllDataPointsForOIDPath("ath_field_to_trigger_2", "ath_form_OID_2", "ath_folder_OID_2", current_subject)[0];

            try
            {
                bool blnTrigForm_1 = CheckDataPoint(Dpt_action);
                bool blnTrigForm_2 = false;
                bool blnTrigForm_3 = CheckDataPoint(oth_dp_2) && oth_dp_2.Data == Tri_str_2;
                if (oth_dps_1 != null && oth_dps_1.Count>0)
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

            //Please note here using Datapoint to tell if a page has been touched
            public bool CheckDataPoint(DataPoint dpt)
            {
                return  dpt != null && dpt.Active && dpt.Data != string.Empty && dpt.ChangeCount > 0 && !dpt.IsBitSet(Status.IsNonConformant);
        }

            //blnTrigForm=true=>add forms
            public void AddForm(Instance cur_ins, string add_formOID, int CRFVersionID)
            {
                DataPage page = cur_ins.DataPages.FindByFormOID(add_formOID);
                Form form = Form.FetchByOID(add_formOID, CRFVersionID);
                if (page == null)
                    cur_ins.AddCRF(form, CRFVersionID);
                else
                {
                    if (page.Active == false)
                        page.Active = true;
                }
            }
            //blnTrigForm =false=>inactivate forms
            public void InactiveForm(Instance cur_ins, string add_formOID)
            {
                DataPage page = cur_ins.DataPages.FindByFormOID(add_formOID);
            //Please note if using page.IsBitSet(Status.IsTouched) to tell a page has been touched or not, 
            //make sure that the page does not have any field with entry restriction for investiators
            if (page != null && page.Active == true && !page.IsBitSet(Status.IsTouched))
            page.Active = false;
            }
       
            }
        }
