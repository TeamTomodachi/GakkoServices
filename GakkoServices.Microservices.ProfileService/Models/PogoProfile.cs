﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.ProfileService.Models
{
    public class PogoProfile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserAccountId { get; set; }
        public Guid PogoTeamId { get; set; }

        public string PogoUsername { get; set; }
        public string PogoTrainerCode { get; set; }
        public int PogoLevel { get; set; }
    }
}