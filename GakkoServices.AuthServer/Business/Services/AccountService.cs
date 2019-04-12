using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GakkoServices.AuthServer.Business.Models;
using GakkoServices.AuthServer.Data.Contexts;
using GakkoServices.AuthServer.Models;
using GakkoServices.AuthServer.Models.Authentication;
using GakkoServices.AuthServer.Models.UserAccount;
using GakkoServices.Core.Messages;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace GakkoServices.AuthServer.Business.Services
{
    public class AccountService
    {
        public enum UsernameLoginMode { Username, Email }
        public UserManager<ApplicationUser> _userManager { get; protected set; }
        public SignInManager<ApplicationUser> _signInManager { get; protected set; }
        public IIdentityServerInteractionService _interaction { get; protected set; }
        public IClientStore _clientStore { get; protected set; }
        public IAuthenticationSchemeProvider _schemeProvider { get; protected set; }
        public IEventService _events { get; protected set; }
        public ILogger _logger { get; protected set; }
        public IBusClient _bus { get; protected set; }
        public IEmailSender _emailSender { get; protected set; }
        public AspIdentityDbContext _identityDbContext { get; protected set; }
        public IServiceScopeFactory _scopeFactory { get; set; }

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            ILoggerFactory loggerFactory,
            IBusClient bus,
            AspIdentityDbContext identityDbContext,
            IServiceScopeFactory scopeFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _logger = loggerFactory?.CreateLogger<AccountService>();
            _bus = bus;
            _identityDbContext = identityDbContext;
            _scopeFactory = scopeFactory;
        }

        public async Task<UserLoginArgs> LoginUser(UserLogin item)
        {
            UsernameLoginMode usernameLoginMode = UsernameLoginMode.Username;
            if (item.Username.Contains('@')) usernameLoginMode = UsernameLoginMode.Email;

            // If the login mode is email, then grab the username for signin purposes
            // string originalUsername = item.Username;
            // if (usernameLoginMode == UsernameLoginMode.Email) {
            //     item.Username = await _identityDbContext.Users
            //         .Where(x => x.NormalizedEmail.ToUpper() == item.Username.ToUpper())
            //         .Select(x => x.UserName)
            //         .FirstOrDefaultAsync();
            // }

            ApplicationUser loggedInUser = null;
            AuthToken token = null;
            var result = await _signInManager.PasswordSignInAsync(item.Username, item.Password, item.RememberLogin, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                loggedInUser = await _userManager.FindByNameAsync(item.Username);
                token = await CreateAuthToken(loggedInUser, true);
                await _events.RaiseAsync(new UserLoginSuccessEvent(loggedInUser.UserName, loggedInUser.Id.ToString(), loggedInUser.UserName));
            }
            else
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(item.Username, "invalid credentials"));
            }

            return new UserLoginArgs(result, loggedInUser, token);
        }

        public async Task<RegisterNewUserArgs> RegisterNewUser(UserCreate item, bool signInUser)
        {
            // Create the User
            AuthToken token = null;
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
                    token = await CreateAuthToken(newUser, true);
                }
            }

            // Return the Successful/Failed Result
            return new RegisterNewUserArgs(result, newUser, token);
        }
        
        public async Task LogoutUserByAuthToken(string authToken, ClaimsPrincipal claimsPrincipalUser) {
            var token = await RetrieveAuthToken(authToken);
            if (token == null) return;

            var user = await _identityDbContext.Users.FindAsync(token.UserId);
            if (claimsPrincipalUser?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(claimsPrincipalUser.GetSubjectId(), claimsPrincipalUser.GetDisplayName()));
            }

            var tokenEntry = _identityDbContext.Remove(token);
            await _identityDbContext.SaveChangesAsync();
        }

        public async Task<ValidateAuthTokenArgs> ValidateAuthToken(string authToken) {
            var token = await RetrieveAuthToken(authToken);
            var currentUtc = DateTime.UtcNow;

            if (token == null || (token.ExpiryDateTimeUtc != null && token.ExpiryDateTimeUtc < currentUtc)) {
                    return new ValidateAuthTokenArgs(null, false);
            } 

            return new ValidateAuthTokenArgs(token, true);
        }

        public async Task<AuthToken> RetrieveAuthToken(string authToken) {
            var token = await _identityDbContext.AuthTokens
                .Where(x => x.Token == authToken)
                .FirstOrDefaultAsync();
            return token;
        }

        public async Task<AuthToken> CreateAuthToken(ApplicationUser user, bool addToDb)
        {
            AuthToken authToken = new AuthToken();
            authToken.LoginDateTimeUtc = DateTime.UtcNow;
            authToken.User = user;
            authToken.UserId = user.Id;
            authToken.ExpiryDateTimeUtc = authToken.LoginDateTimeUtc + TimeSpan.FromDays(7);
            authToken.Token = HashString($"{user.Id}{authToken.LoginDateTimeUtc}{user.SecurityStamp}");

            if (addToDb) {
                await _identityDbContext.AddAsync(authToken);
                await _identityDbContext.SaveChangesAsync();
            }
            
            return authToken;
        }

        private string HashString(string item) 
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[64 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
 
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: item,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
