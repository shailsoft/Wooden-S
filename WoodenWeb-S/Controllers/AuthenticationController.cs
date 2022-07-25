using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WoodenWeb_S.Models;

namespace WoodenWeb_S.Controllers
{
    [Authorize]
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IConfiguration configure;
        private readonly string apiBaseUrl = string.Empty;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string jwtToken = string.Empty;

        public AuthenticationController(ILogger<AuthenticationController> _logger, IConfiguration _configuration, IHttpContextAccessor _httpContextAccessor)
        {
            logger = _logger;
            configure = _configuration;
            httpContextAccessor = _httpContextAccessor;
            apiBaseUrl = "http://localhost:25228/api/";// configure.GetValue<string>("http://localhost:25228/api/");

            //To get the claim details
            ClaimsIdentity identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claims = identity.Claims.ToList();
            if (claims != null && claims.Any())
            {
                jwtToken = claims.Where(c => c.Type == "Token").FirstOrDefault().Value;
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using HttpClient client = new();
                    logger.LogInformation("Call the Login Method");
                    StringContent content = new(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
                    string endpoint = apiBaseUrl + "account/login";
                    using var loginResponse = await client.PostAsync(endpoint, content);

                    if (loginResponse.StatusCode == HttpStatusCode.OK)
                    {
                        logger.LogInformation("Call the Login Method Success" + Response.StatusCode);
                        var contents = loginResponse.Content.ReadAsStringAsync();
                        var jsonResponse = contents.Result;

                        JWTTokenResponse jWTTokenResponse = JsonConvert.DeserializeObject<JWTTokenResponse>(jsonResponse);
                        if (jWTTokenResponse != null)
                        {
                            IEnumerable<Claim> claims = new List<Claim>() {
                                new Claim(ClaimTypes.Name, loginModel.Email),
                                new Claim(ClaimTypes.Email, loginModel.Email),
                                new Claim("Token", jWTTokenResponse.Token),
                                new Claim("TokenExpiry", jWTTokenResponse.Expiration.ToString())
                            };

                            //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme   
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity   
                            var principal = new ClaimsPrincipal(identity);

                            var props = new AuthenticationProperties
                            {
                                IsPersistent = true,
                            };

                            // to register the cookie to the browser
                            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                            //SignInAsync is a Extension method for Sign in a principal for the specified scheme.   
                            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties());

                            return RedirectToAction("Index", "Dashboard");
                        }
                    }

                    else if (loginResponse.StatusCode == HttpStatusCode.Unauthorized || loginResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        ModelState.Clear();

                        ModelState.AddModelError(string.Empty, "Invalid Credentials");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, Convert.ToString(loginResponse.StatusCode));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Call the Login Method Exception" + ex.ToString());
            }
            return View();
        }

    }
}
