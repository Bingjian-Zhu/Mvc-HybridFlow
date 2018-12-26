using AuthServer.Models;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer
{
    public class ProfileService : IProfileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProfileService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        ////services
        //private readonly IUserRepository _userRepository;

        //public ProfileService(IUserRepository userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        //Get user profile date in terms of claims when calling /connect/userinfo
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                //depending on the scope accessing the user data.
                if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                {
                    var client = _httpClientFactory.CreateClient();
                    //已过时
                    DiscoveryResponse disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                    TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "AuthServer", "secret");
                    var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

                    //var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    //{
                    //    Address = "http://localhost:5000",
                    //    ClientId = "AuthServer",
                    //    ClientSecret = "secret",
                    //    Scope = "api1"
                    //});
                    //if (TokenResponse.IsError) throw new Exception(TokenResponse.Error);
                    client.SetBearerToken(tokenResponse.AccessToken);

                    var response = await client.GetAsync("http://localhost:5001/api/values/" + context.Subject.Identity.Name);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Resource server is not working!");
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        User user = JsonConvert.DeserializeObject<User>(content);
                        //get user from db (in my case this is by email)
                        //var user = await _userRepository.FindAsync(context.Subject.Identity.Name);
                        if (user != null)
                        {
                            //获取user中的Claims
                            var claims = GetUserClaims(user);
                            //set issued claims to return
                            //context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                            context.IssuedClaims = claims.ToList();
                        }
                    }
                }
                else
                {
                    //get subject from context (this was set ResourceOwnerPasswordValidator.ValidateAsync),
                    //where and subject was set to my user id.
                    var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");
                    //获取User_Id
                    if (!string.IsNullOrEmpty(userId?.Value) && long.Parse(userId.Value) > 0)
                    {
                        var client = _httpClientFactory.CreateClient();
                        //已过时
                        DiscoveryResponse disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                        TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "AuthServer", "secret");
                        var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

                        //var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                        //{
                        //    Address = "http://localhost:5000",
                        //    ClientId = "AuthServer",
                        //    ClientSecret = "secret",
                        //    Scope = "api1"
                        //});
                        //if (TokenResponse.IsError) throw new Exception(TokenResponse.Error);
                        client.SetBearerToken(tokenResponse.AccessToken);

                        //根据User_Id获取user
                        var response = await client.GetAsync("http://localhost:5001/api/values/" + long.Parse(userId.Value));
                        //get user from db (find user by user id)
                        //var user = await _userRepository.FindAsync(long.Parse(userId.Value));
                        var content = await response.Content.ReadAsStringAsync();
                        User user = JsonConvert.DeserializeObject<User>(content);
                        // issue the claims for the user
                        if (user != null)
                        {
                            //获取user中的Claims
                            var claims = GetUserClaims(user);
                            //context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                            context.IssuedClaims = claims.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log your error
            }
        }

        //check if user account is active.
        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                {
                    var client = _httpClientFactory.CreateClient();
                    //已过时
                    DiscoveryResponse disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                    TokenClient tokenClient = new TokenClient(disco.TokenEndpoint, "AuthServer", "secret");
                    var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

                    //var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    //{
                    //    Address = "http://localhost:5000",
                    //    ClientId = "AuthServer",
                    //    ClientSecret = "secret",
                    //    Scope = "api1"
                    //});
                    //if (TokenResponse.IsError) throw new Exception(TokenResponse.Error);
                    client.SetBearerToken(tokenResponse.AccessToken);

                    var response = await client.GetAsync("http://localhost:5001/api/values/" + context.Subject.Identity.Name);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Resource server is not working!");
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        User user = JsonConvert.DeserializeObject<User>(content);
                        //get subject from context (set in ResourceOwnerPasswordValidator.ValidateAsync),
                        var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

                        if (!string.IsNullOrEmpty(userId?.Value) && long.Parse(userId.Value) > 0)
                        {
                            //var user = await _userRepository.FindAsync(long.Parse(userId.Value));

                            if (user != null)
                            {
                                if (user.IsActive)
                                {
                                    context.IsActive = user.IsActive;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //handle error logging
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
