using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.AuthServer.Business.Models;
using GakkoServices.AuthServer.Models;
using GakkoServices.AuthServer.Models.Authentication;
using GakkoServices.AuthServer.Models.UserAccount;
using GakkoServices.Core.Messages;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace GakkoServices.AuthServer.Business.Services
{
    public class AccountService
    {
        public UserManager<ApplicationUser> _userManager { get; protected set; }
        public SignInManager<ApplicationUser> _signInManager { get; protected set; }
        public IIdentityServerInteractionService _interaction { get; protected set; }
        public IClientStore _clientStore { get; protected set; }
        public IAuthenticationSchemeProvider _schemeProvider { get; protected set; }
        public IEventService _events { get; protected set; }
        public ILogger _logger { get; protected set; }
        public IBusClient _bus { get; protected set; }
        public IEmailSender _emailSender { get; protected set; }

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            ILoggerFactory loggerFactory,
            IBusClient bus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _logger = loggerFactory?.CreateLogger<AccountService>();
            _bus = bus;
        }

        public async Task<UserLoginArgs> LoginUser(UserLogin item)
        {
            ApplicationUser loggedInUser = null;
            var result = await _signInManager.PasswordSignInAsync(item.Username, item.Password, item.RememberLogin, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                loggedInUser = await _userManager.FindByNameAsync(item.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(loggedInUser.UserName, loggedInUser.Id.ToString(), loggedInUser.UserName));
            }

            return new UserLoginArgs(result, loggedInUser);
        }

        public async Task<RegisterNewUserArgs> RegisterNewUser(UserCreate item, bool signInUser)
        {
            // Create the User
            var newUser = new ApplicationUser { UserName = item.Username, Email = item.Email };
            var result = await _userManager.CreateAsync(newUser, item.Password);
            if (result.Succeeded)
            {
                // Log the User Creation
                _logger.LogInformation(3, $"User({newUser.Id}): ${newUser.UserName}, created a new account with password.");
                await _bus.PublishAsync<UserCreateMessage>(new UserCreateMessage { Id = newUser.Id });

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                // Sign in the new User, log that their account was created...
                if (signInUser)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                }
            }

            // Return the Successful/Failed Result
            return new RegisterNewUserArgs(result, newUser);
        }
    }
}
