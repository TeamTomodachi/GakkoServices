using System;
using System.Collections.Generic;

namespace GakkoServices.APIGateway.Models
{
    public class Profile
    {
        public string Username { get; set; }
        public string TrainerCode { get; set; }
        public int Level { get; set; }
        public Team Team { get; set; }
        public List<Pokemon> Pokemon { get; set; }
    }
}