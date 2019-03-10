using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.MetadataService.Models
{
    public class PogoPokemon
    {
        [Key]
        public Guid Id { get; set; }
        public int PokedexNumber { get; set; }
        public string PokemonName { get; set; }
    }
}
