using System;
using System.Collections.Generic;
using System.Text;

namespace GakkoServices.Core.Models
{
    /// <summary>
    /// A container class for SecretAppSettings
    /// </summary>
    /// <example>
    /// An example json for file for `secretappsettings.json`
    /// {
    ///     "dbUsername": "",
    ///     "dbPassword": "",
    ///     "dbConnectionString": "",
    ///     "dbServerEngine": ""
    /// }
    /// </example>
    public class SecretAppSettings
    {
        public string DBUsername { get; set; }
        public string DBPassword { get; set; }
        public string DBConnectionString { get; set; }
        public string DBServerEngine { get; set; }
    }
}
