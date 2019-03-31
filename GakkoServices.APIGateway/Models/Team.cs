using System;

namespace GakkoServices.APIGateway.Models
{
    public class Team {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ImageUrl { get; set; }
    }
}
