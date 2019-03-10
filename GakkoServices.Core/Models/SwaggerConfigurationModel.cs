using System;
using System.Collections.Generic;
using System.Text;

namespace GakkoServices.Core.Models
{
    public class SwaggerConfigurationModel
    {
        public string Title { get; set; } = "API";
        public string Version { get; set; } = "v1";
        public string Name { get; set; } = "API";
        public string SwaggerEndpoint { get; set; } = "/swagger/v1/swagger.json";

    }
}
