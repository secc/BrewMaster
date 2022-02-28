using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SECC.Rock.Authentication;
using SECC.Rock.Contracts;
using SECC.Rock.OAuth;
using SECC.Rock.Identity;

namespace BrewMasterWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRockOAuthClient _rockOAuthClient;

        public HomeController(ILogger<HomeController> logger, IRockOAuthClient rockOAuthClient)
        {
            _logger = logger;
            _rockOAuthClient = rockOAuthClient;
        }

        
        public async Task<IActionResult> Index()
        {
            RockPerson rockPerson = await _rockOAuthClient.GetRockPerson( HttpContext );

            if (rockPerson != null)
            {
                var authenticateResult = await HttpContext.AuthenticateAsync();
                ViewBag.Roles = authenticateResult.Principal.Claims.Where(c => c.Type == "Role").Select(c => c.Value);
            }

            return View(rockPerson);
        }

        [Authorize(ClaimsPolicies.Edit)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
