using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GakkoServices.Core.Helpers
{
    public static class ReflectionHelpers
    {
        /// <summary>
        /// Retrieves the Version Number from the specified assembly
        /// </summary>
        public static Version GetVersionNumber(Assembly assembly) {
            if (assembly == null) assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;
            return version;
        }
        
        /// <summary>
        /// Retrieves the Version Number from the specified assembly using the default .NET Wildcard rules
        /// </summary>
        public static void GetVersionNumber(Assembly assembly, out Version version, out DateTime buildDate) {
            
            version = GetVersionNumber(assembly);
            buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);
        }
        
        /// <summary>
        /// https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection-in-c-sharp
        /// </summary>
        public static object GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection-in-c-sharp
        /// </summary>
        public static T GetPropValue<T>(this Object obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
