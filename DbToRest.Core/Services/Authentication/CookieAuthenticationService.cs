using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DbToRest.Core.Services.Authentication
{
    /// <summary>
    /// Represents service using cookie middleware for the authentication
    /// </summary>
    public partial class CookieAuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;

        private SessionUser _cachedSessionUser;

        #endregion Fields

        #region Ctor

        public CookieAuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="isPersistent">Whether the authentication session is persisted across multiple requests</param>
        public virtual async void SignIn(SessionUser sessionUser, bool isPersistent)
        {
            if (sessionUser == null)
                throw new ArgumentNullException(nameof(sessionUser));

            //create claims for customer's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(sessionUser.Username))
                claims.Add(new Claim(ClaimTypes.Name, sessionUser.Username, ClaimValueTypes.String, DbToRestAuthenticationDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(sessionUser.Email))
                claims.Add(new Claim(ClaimTypes.Email, sessionUser.Email, ClaimValueTypes.Email, DbToRestAuthenticationDefaults.ClaimsIssuer));

            claims.Add(new Claim(ClaimTypes.Sid, sessionUser.Id.ToString(), ClaimValueTypes.String, DbToRestAuthenticationDefaults.ClaimsIssuer));


            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, DbToRestAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync(DbToRestAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

            //cache authenticated customer
            _cachedSessionUser = sessionUser;
        }

        /// <summary>
        /// Sign out
        /// </summary>
        public virtual async void SignOut()
        {
            //reset cached customer
            _cachedSessionUser = null;

            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync(DbToRestAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Get authenticated customer
        /// </summary>
        /// <returns>Customer</returns>
        public virtual SessionUser GetAuthenticatedCustomer()
        {
            //whether there is a cached customer
            if (_cachedSessionUser != null)
                return _cachedSessionUser;

            //try to get authenticated user identity
            var authenticateResult = _httpContextAccessor.HttpContext.AuthenticateAsync(DbToRestAuthenticationDefaults.AuthenticationScheme).Result;
            if (!authenticateResult.Succeeded)
                return null;

            SessionUser sessionUser = new SessionUser();

            //try to get customer by username
            var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                && claim.Issuer.Equals(DbToRestAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));

            var userIdClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Sid
       && claim.Issuer.Equals(DbToRestAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));

            if (usernameClaim != null)
            {
                sessionUser.Username = usernameClaim.Value;
                sessionUser.Id = new Guid(userIdClaim.Value);
            }

            /*else
            {
                //try to get customer by email
                var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email
                    && claim.Issuer.Equals(DbToRestAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (emailClaim != null)
                    sessionUser = _customerService.GetCustomerByEmail(emailClaim.Value);
            }*/

            //whether the found customer is available
            if (sessionUser == null /*|| !sessionUser.Active || sessionUser.RequireReLogin || sessionUser.Deleted || !sessionUser.IsRegistered()*/)
                return null;

            //cache authenticated customer
            _cachedSessionUser = sessionUser;

            return _cachedSessionUser;
        }

        #endregion Methods
    }
}