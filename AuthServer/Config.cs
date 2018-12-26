using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(
                name: "mvc.profile",
                displayName: "Mvc profile",
                claimTypes: new[] { "role" });
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                //new IdentityResource("roles","role",new List<string>{ "role"}),
                customProfile
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API",new List<string>(){ "role"})
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "AuthServer",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" },
                    Claims= new List<Claim>(){new Claim("role","AuthServer") },
                    ClientClaimsPrefix = ""
                },
                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                   ClientId = "mvc",
                   ClientName = "MVC Client",
                   AllowedGrantTypes = GrantTypes.Hybrid,
                   ClientSecrets =
                   {
                       new Secret("secret".Sha256())
                   },
                   // where to redirect to after login
                   RedirectUris = { "http://localhost:5002/signin-oidc" },

                   // where to redirect to after logout
                   PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                  AllowedScopes = new List<string>
                  {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    //"roles",
                    "mvc.profile"
                  }
                }
            };
        }
    }
}
