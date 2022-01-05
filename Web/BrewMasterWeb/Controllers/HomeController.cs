using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWeb.Models;
using BrewMasterWeb.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BrewMasterWeb.Utilities;

namespace BrewMasterWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRockAuthService _rockAuthService;

        public HomeController(ILogger<HomeController> logger, IRockAuthService rockAuthService)
        {
            _logger = logger;
            _rockAuthService = rockAuthService;
        }

        
        public async Task<IActionResult> Index()
        {
            RockPerson rockPerson = await _rockAuthService.GetRockPersonAsync(HttpContext);

            if (rockPerson != null)
            {
                var authenticateResult = await HttpContext.AuthenticateAsync();
                ViewBag.Roles = authenticateResult.Principal.Claims.Where(c => c.Type == "Role").Select(c => c.Value);
            }

            return View(rockPerson);
        }

        [Authorize(ClaimsPolicies.Administrator)]
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
