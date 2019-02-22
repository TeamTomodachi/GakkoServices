using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GakkoServices.Core.Helpers
{
    public static class ExceptionHelpers
    {
        public static string PrintOutException(this Exception ex, string headerMessage)
        {
            string str = headerMessage;
            try
            {
                Debug.WriteLine(headerMessage);
                str += "\nSource: " + ex.Source +
                        "\n Help Link: " + ex.HelpLink +
                        "\n HResult: " + ex.HResult +
                        "\n Message: " + ex.Message +
                        "\n StackTrace: " + ex.StackTrace;
                Debug.WriteLine(str);

                foreach (var key in ex.Data.Keys)
                {
                    Debug.WriteLine(key.ToString() + " | " + ex.Data[key].ToString());
                }

                str += "\n";

                if (ex.InnerException != null)
                    str += PrintOutException(ex.InnerException, "Entering Inner Exception");
            }
            catch (Exception) { str = headerMessage; }

            return str;
        }
    }
}
