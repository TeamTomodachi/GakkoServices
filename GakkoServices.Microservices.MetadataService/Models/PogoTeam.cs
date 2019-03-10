using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.MetadataService.Models
{
    public class PogoTeam
    {
        [Key]
        public Guid Id { get; set; }
        public string TeamName { get; set; }
        public string TeamColor { get; set; }
    }
}
