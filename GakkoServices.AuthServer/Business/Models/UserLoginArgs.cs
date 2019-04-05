using GakkoServices.AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.AuthServer.Business.Models
{
    public class UserLoginArgs
    {
        public SignInResult Result { get; set; }
        public ApplicationUser LoggedInUser { get; set; }
        public AuthToken Token { get; set; }
        public bool Successful { get { return Result.Succeeded; } }

        public UserLoginArgs(SignInResult result, ApplicationUser loggedInUser, AuthToken token)
        {
            Result = result;
            LoggedInUser = loggedInUser;
            Token = token;
        }
    }
}
