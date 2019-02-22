using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Core.Helpers
{
    public static class EnumHelpers
    {
        public static IEnumerable<KeyValuePair<string, int>> GetNameValuePair<T>()
        {
            List<KeyValuePair<string, int>> enums = new List<KeyValuePair<string, int>>();

            Type genericType = typeof(T);
            foreach (T obj in Enum.GetValues(genericType))
            {
                Enum test = Enum.Parse(typeof(T), obj.ToString()) as Enum;
                int x = Convert.ToInt32(test);
                enums.Add(new KeyValuePair<string, int>(obj.ToString(), x));
            }

            return enums;
        }
        
        public static int Count(this Enum e)
        {
            var names = Enum.GetNames(e.GetType());
            return names.Length;
        }
    }
}
