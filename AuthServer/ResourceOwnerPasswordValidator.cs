using AuthServer.Models;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ResourceOwnerPasswordValidator(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                //已过时
                //DiscoveryResponse disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                //TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "AuthServer", "secret");
                //var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

                DiscoveryResponse disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "AuthServer",
                    ClientSecret = "secret",
                    Scope = "api1"
                });
                if (tokenResponse.IsError)
                    throw new Exception(tokenResponse.Error);

                client.SetBearerToken(tokenResponse.AccessToken);
                var response = await client.GetAsync("http://localhost:5001/api/values/" + context.UserName + "/" + context.Password);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Resource server is not working!");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(content);
                    //get your user model from db (by username - in my case its email)
                    //var user = await _userRepository.FindAsync(context.UserName);
                    if (user != null)
                    {
                        //check if password match - remember to hash password if stored as hash in db
                        if (user.Password == context.Password)
                        {
                            //set the result
                            context.Result = new GrantValidationResult(
                                subject: user.UserId.ToString(),
                                authenticationMethod: "custom",
                                claims: GetUserClaims(user));

                            return;
                        }
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                        return;
                    }
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                    return;
                }
            }
            catch (Exception ex)
            {

            }

        }
        public static Claim[] GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>();
            Claim claim;
            foreach (var itemClaim in user.Claims)
            {
                claim = new Claim(itemClaim.Type, itemClaim.Value);
                claims.Add(claim);
            }
            return claims.ToArray();
        }
    }
}
