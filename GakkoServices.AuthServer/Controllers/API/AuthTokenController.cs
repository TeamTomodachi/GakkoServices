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
using Newtonsoft.Json;

namespace GakkoServices.AuthServer.Controllers
{
    // [EnableCors(Startup.CORS_POLICY)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTokenController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public AuthTokenController(AccountService accountService, ILoggerFactory loggerFactory)
        {
            _accountService = accountService;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AuthTokenController>();
        }

        /// <summary>
        /// Validates a given AuthToken is active
        /// </summary>
        /// <param name="token">An Authentication Token</param>
        /// <returns>Whether the AuthToken is valid</returns>
        // [EnableCors(Startup.CORS_POLICY)]
        [HttpPost]
        public async Task<IActionResult> ValidateToken([FromHeader] string token) {
            var validationArgs = await _accountService.ValidateAuthToken(token);
            if (!validationArgs.IsValid) {
                validationArgs.Token = null;
            }

            dynamic d = new {
                IsValid = validationArgs.IsValid,
                Token = validationArgs?.Token.Token,
                LoginDateTimeUtc = validationArgs?.Token.LoginDateTimeUtc,
                ExpiryDateTimeUtc = validationArgs?.Token.ExpiryDateTimeUtc,
            };
            return new ObjectResult(d);
        }
    }
}