using System;
using System.Collections.Generic;
using System.Text;

namespace GakkoServices.Core.Helpers
{
    public class EncryptionHelper
    {
        public static string EncodeBase64String(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText)) return "";
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeBase64String(string base64EncodedData)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedData)) return "";
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static bool IsBase64String(string base64EncodedData)
        {
            try
            {
                var decodedString = DecodeBase64String(base64EncodedData);
                return true;
            }
            catch (Exception) { return false; }
        }

        public static bool IsBase64String(string base64EncodedData, out string decodedString)
        {
            try
            {
                decodedString = DecodeBase64String(base64EncodedData);
                return true;
            }
            catch (Exception)
            {
                decodedString = "";
                return false;
            }
        }
    }
}
