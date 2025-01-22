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
    // using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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


            *
            @ Classification: xxxx (Edit Check / Derivation / Dynamism)
            @ Introduction: RAVE custom function template
            @ Dpt_action: xxxx
            @ Note: xxxx
            */
            try
            {
                ActionFunctionParams afp = (ActionFunctionParams) ThisObject;
                DataPoint Dpt_action = afp.ActionDataPoint;
                Subject current_subject = Dpt_action.Record.Subject;
            
                // wite main CF code below
            
            }
            catch
            {
            }
            return null;

        }
    }
}
