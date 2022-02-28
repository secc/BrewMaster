using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SECC.Rock.OAuth;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BrewMasterWeb.Controllers
{
    public class Auth : Controller
    {
        private readonly IRockOAuthClient _rockAuthService;

        public Auth( IRockOAuthClient rockAuthService )
        {
            _rockAuthService = rockAuthService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var auth = await HttpContext.AuthenticateAsync();

            return Json( "test" );
        }


        public IActionResult Login()
        {
            return _rockAuthService.RedirectToIdP();
        }

        public async Task<IActionResult> Callback()
        {
            var roles = await _rockAuthService.Authenticate( HttpContext );

            return Redirect( "/" );
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect( "/" );
        }

    }



}
