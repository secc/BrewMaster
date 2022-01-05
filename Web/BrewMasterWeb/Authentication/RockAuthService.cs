using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BrewMasterWeb.Authentication;
using BrewMasterWeb.Utilities;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace BrewMasterWeb.Authentication
{
    public class RockAuthService : IRockAuthService
    {
        readonly private string _authorizationURI;
        readonly private string _tokenURI;
        readonly private string _userLoginEndpoint;
        readonly private string _clientId;
        readonly private string _clientSecret;
        readonly private string _apiKey;

        public RockAuthService(IConfiguration configuration)
        {
            var options = configuration.GetSection("RockAuth").Get<RockAuthOptions>();
            _authorizationURI = options.AuthorizationURI;
            _tokenURI = options.TokenURI;
            _userLoginEndpoint = options.UserLoginEndpoint;
            _clientId = options.ClientId;
            _clientSecret = options.ClientSecret;
            _apiKey = options.ApiKey;
        }

        public async Task<bool> Authenticate(HttpContext httpContext)
        {
            string code = GetCode(httpContext);
            TokenResponse tokenResponse = await GetTokens(code);
            var person = await GetPerson(tokenResponse.AccessToken);
            await AuthenticateUser(person, httpContext);

            return false;
        }

        public async Task<RockPerson> GetRockPersonAsync(HttpContext httpContext)
        {
            var auth = await httpContext.AuthenticateAsync();
            var name = auth?.Principal?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization-Token", _apiKey);
            var response = await httpClient.GetAsync("https://rock.secc.org/api/People/GetByPersonAliasId/" + name);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RockPerson>(content);
        }


        private async Task AuthenticateUser(RockPerson person, HttpContext httpContext)
        {
            var template = $@"{{% assign groupMembers = {person.Id} | PersonById | Groups: '1', 'Active'  -%}}
[
{{% for groupMember in groupMembers -%}}
    ""{{{{ groupMember.Group.Name }}}}""{{% if forloop.last == false %}},{{% endif %}}
{{% endfor -%}}
]";
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization-Token", _apiKey);
            var response = await httpClient.PostAsync("https://rock.secc.org/api/Lava/RenderTemplate", new StringContent(template));
            var content = await response.Content.ReadAsStringAsync();

            content = content.Substring(1, content.Length - 2).Replace("\\\"", "\"").Replace("\\n", "");

            var claimItems = JsonConvert.DeserializeObject<List<string>>(content);

            var claims = new List<Claim>
            {
                new Claim("Role", ClaimsRoles.User),
                new Claim("Name", $"{person.FirstName} {person.LastName}")
            };

            foreach (var claim in claimItems)
            {
                claims.Add(new Claim("Role", claim));
            }

            var identity = new RockIdentity(person.PrimaryAliasId.ToString());
            await httpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(identity, claims, "Cookie", "user", "role")));
        }

        private async Task<RockPerson> GetPerson(string accessToken)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync("https://rock.secc.org/api/People/GetCurrentPerson");
            var content = await response.Content.ReadAsStringAsync();

            var person = JsonConvert.DeserializeObject<RockPerson>(content);
            return person;
        }

        private async Task<TokenResponse> GetTokens(string code)
        {
            var keysCombined = System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}");
            var auth = System.Convert.ToBase64String(keysCombined);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

                var dict = new Dictionary<string, string>();
                dict.Add("code", code);
                dict.Add("grant_type", "authorization_code");
                dict.Add("redirect_uri", _userLoginEndpoint);

                var request = new HttpRequestMessage(HttpMethod.Post, _tokenURI)
                {
                    Content = new FormUrlEncodedContent(dict)
                };

                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                var tokens = JsonConvert.DeserializeObject<TokenResponse>(content);

                return tokens;
            }
        }

        private string GetCode(HttpContext httpContext)
        {
            if (!httpContext.Request.Query.ContainsKey("code"))
            {
                throw new Exception("OAuth code missing from response");
            }

            return httpContext.Request.Query["code"];
        }

        public IActionResult RedirectToIdP()
        {
            var url = $"{_authorizationURI}?response_type=code&client_id={_clientId}&redirect_uri={_userLoginEndpoint}";

            return new RedirectResult(url);
        }
    }

    public class RockAuthOptions
    {
        public string AuthorizationURI { get; set; }
        public string TokenURI { get; set; }
        public string UserLoginEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }
    }


    public static class RockAuthServiceHelper
    {
        public static IServiceCollection AddRockAuthService(this IServiceCollection services)
        {
            services.AddScoped<IRockAuthService, RockAuthService>();
            return services;
        }
    }
}
