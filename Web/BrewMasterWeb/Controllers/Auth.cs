using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BrewMasterWeb.Authentication;
using BrewMasterWeb.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BrewMasterWeb.Controllers
{
    public class Auth : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IRockAuthService _rockAuthService;

        public Auth(IConfiguration configuration, IRockAuthService rockAuthService)
        {
            _configuration = configuration;
            _rockAuthService = rockAuthService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var auth = await HttpContext.AuthenticateAsync();
            var claims = new List<Claim>
            {
                new Claim("Role", "Admin")
            };
            return Json("test");
        }


        public IActionResult Login()
        {
            return _rockAuthService.RedirectToIdP();
        }

        public async Task<IActionResult> Callback()
        {
            
            bool success = await _rockAuthService.Authenticate(HttpContext);

            return Json("Success"); ;
        }

    }



}
