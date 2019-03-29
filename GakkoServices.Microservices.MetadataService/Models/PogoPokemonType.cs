using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.MetadataService.Models
{
    public class PogoPokemonType
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
