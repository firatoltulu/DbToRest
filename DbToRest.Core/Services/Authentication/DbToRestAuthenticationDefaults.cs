using Microsoft.AspNetCore.Http;

namespace DbToRest.Core.Services.Authentication
{
    /// <summary>
    /// Represents default values related to authentication services
    /// </summary>
    public static partial class DbToRestAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for authentication scheme
        /// </summary>
        public static string AuthenticationScheme => "Authentication";

        /// <summary>
        /// The default value used for external authentication scheme
        /// </summary>
        public static string ExternalAuthenticationScheme => "ExternalAuthentication";

        /// <summary>
        /// The issuer that should be used for any claims that are created
        /// </summary>
        public static string ClaimsIssuer => "dbtorestapp";

        /// <summary>
        /// The default value for the login path
        /// </summary>
        public static PathString LoginPath => new PathString("/login");

        /// <summary>
        /// The default value used for the logout path
        /// </summary>
        public static PathString LogoutPath => new PathString("/logout");

        /// <summary>
        /// The default value for the access denied path
        /// </summary>
        public static PathString AccessDeniedPath => new PathString("/page-not-found");

        /// <summary>
        /// The default value of the return URL parameter
        /// </summary>
        public static string ReturnUrlParameter => string.Empty;

        /// <summary>
        /// Gets a key to store external authentication errors to session
        /// </summary>
        public static string ExternalAuthenticationErrorsSessionKey => "dbtorest.externalauth.errors";
    }
}