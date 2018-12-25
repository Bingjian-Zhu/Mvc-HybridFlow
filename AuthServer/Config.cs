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

        //public static List<TestUser> GetUsers()
        //{
        //    return new List<TestUser>
        //    {
        //        new TestUser
        //        {
        //            SubjectId = "1",
        //            Username = "test",
        //            Password = "123",

        //            Claims = new List<Claim>
        //            {
        //                new Claim("role", "user")
        //            }
        //        },
        //        new TestUser
        //        {
        //            SubjectId = "2",
        //            Username = "admin",
        //            Password = "123",

        //            Claims = new List<Claim>
        //            {
        //                new Claim("role", "admin")
        //            }
        //        }
        //    };
        //}

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles","role",new List<string>{ "role"})
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
                    Claims= new List<Claim>(){new Claim(JwtClaimTypes.Role,"AuthServer") },
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
                    "roles"
                  }
                }
            };
        }
    }
}
