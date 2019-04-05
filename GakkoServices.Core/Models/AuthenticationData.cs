using System;

namespace GakkoServices.Core.Models
{
    public class AuthenticationData
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }

        public int TokenId { get; set; }
        public string Token { get; set; }
    }
}
