using System;
using System.Collections.Generic;
using System.Text;

namespace GakkoServices.Core.Helpers
{
    public static class CurrencyHelpers
    {
        public static string ConvertToShorthand(double value)
        {
            string stringValue = value.ToString();

            if (value > 100000000000) return stringValue.Substring(0, 3) + "B";
            if (value > 10000000000) return stringValue.Substring(0, 2) + "B";
            if (value > 1000000000) return stringValue.Substring(0, 1) + "B";
            if (value > 100000000) return stringValue.Substring(0, 3) + "M";
            if (value > 10000000) return stringValue.Substring(0, 2) + "M";
            if (value > 1000000) return stringValue.Substring(0, 1) + "M";
            if (value > 100000) return stringValue.Substring(0, 3) + "K";
            if (value > 10000) return stringValue.Substring(0, 2) + "K";
            if (value > 1000) return stringValue.Substring(0, 1) + "K";

            if (stringValue.Length > 3)
                return stringValue.Substring(0, 3);
            else
                return stringValue;
        }

        public static string ConvertBackToRegularForm(string value)
        {
            throw new NotImplementedException();
        }
    }
}
