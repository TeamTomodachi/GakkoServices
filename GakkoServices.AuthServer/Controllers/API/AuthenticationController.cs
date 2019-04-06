using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.AuthServer.Business.Services;
using GakkoServices.AuthServer.Models;
using GakkoServices.AuthServer.Models.Authentication;
using GakkoServices.AuthServer.Models.UserAccount;
using GakkoServices.Core.Helpers;
using IdentityServer4.Events;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GakkoServices.AuthServer.Controllers
{
    // [EnableCors(Startup.CORS_POLICY)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public AuthenticationController(AccountService accountService, ILoggerFactory loggerFactory)
        {
            _accountService = accountService;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AuthenticationController>();
        }

        /// <summary>
        /// Attempts to sign a user into the system
        /// </summary>
        /// <param name="item">A UserLogin containing Username and Password</param>
        /// <returns>The user login token or errors</returns>
        // [EnableCors(Startup.CORS_POLICY)]
        [HttpPost]
        public async Task<IActionResult> LoginCredentials([FromBody] UserLogin item)
        {
            var loginResult = await _accountService.LoginUser(item);
            if (loginResult.Successful)
            {
                var user = loginResult.LoggedInUser;
                var userClaims = await _accountService._userManager.GetClaimsAsync(user);
                dynamic d = new {
                    message=$"User has successfully logged in",
                    claims=userClaims,
                    token=loginResult.Token.Token,
                    expiryDate=loginResult.Token.ExpiryDateTimeUtc,
                    loginDate=loginResult.Token.LoginDateTimeUtc
                };
                return new ObjectResult(d);
            }

            return new ObjectResult(AccountOptions.InvalidCredentialsErrorMessage);
        }

        /// <summary>
        /// Validates a given AuthToken is active
        /// </summary>
        /// <param name="item">An Authentication Token</param>
        /// <returns>Whether the AuthToken is valid</returns>
        // [EnableCors(Startup.CORS_POLICY)]
        [HttpPost]
        public async Task<IActionResult> ValidateToken([FromHeader] string token) {
            var validationArgs = await _accountService.ValidateAuthToken(token);
            if (!validationArgs.IsValid) {
                validationArgs.Token = null;
            }
            return new ObjectResult(validationArgs);
        }
    }
}