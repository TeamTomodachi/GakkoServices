using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.AuthServer.Models
{
    public class AuthToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime LoginDateTimeUtc { get; set; }

        [Required]
        public DateTime ExpiryDateTimeUtc { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

    }
}
