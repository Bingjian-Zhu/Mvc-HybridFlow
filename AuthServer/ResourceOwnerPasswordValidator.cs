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
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                DiscoveryResponse disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "AuthServer", "secret");
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
                var client = new HttpClient();
                client.SetBearerToken(tokenResponse.AccessToken);
                try
                {
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
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
                }
            }
            catch (Exception ex)
            {

            }

        }
        public static Claim[] GetUserClaims(User user)
        {
            var claims = new Claim[user.Claims.Count];
            //roles
            int index = 0;
            foreach (var itemClaim in user.Claims)
            {
                claims[index] = new Claim(JwtClaimTypes.Role, itemClaim.Value);
                index++;
            }
            return claims;
        }
    }
}
