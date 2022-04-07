using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrewMasterWeb.Data;
using BrewMasterWeb.Models;
using BrewMasterWeb.Services;
using BrewMasterWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SECC.Rock.Authentication;
using SECC.Rock.RockInterface;

namespace BrewMasterWeb.Controllers
{
    public class APIController : Controller
    {

        ICoffeeMakerService _coffeeMakerService;
        IRockAuthService _rockAuthService;
        IRockClient _rockClient;

        public APIController( ICoffeeMakerService coffeeMakerService, IRockAuthService rockAuthService, IRockClient rockClient )
        {
            _coffeeMakerService = coffeeMakerService;
            _rockAuthService = rockAuthService;
            _rockClient = rockClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CoffeeMakers()
        {
            var coffeeMakers = await _coffeeMakerService.All();
            return Json( coffeeMakers );
        }

        public async Task<IActionResult> CoffeeMakersForPerson(string personToken )
        {
            if (!await _rockAuthService.CanAdministrate(personToken))
            {
                throw new Exception( "No!" );
            }

            List<CoffeeMakerPersonViewModel> coffeeMakers = await _coffeeMakerService.ForPerson( personToken );
            return Json( coffeeMakers );
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe( [FromForm] CoffeeMakerSubscribeViewModel makerSubscribe )
        {
            await _coffeeMakerService.Subscribe( makerSubscribe );

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe( [FromForm] CoffeeMakerSubscribeViewModel makerSubscribe )
        {
            await _coffeeMakerService.Unsubscribe( makerSubscribe );

            return NoContent();
        }

        public async Task<IActionResult> CanAdministrate( string personToken )
        {
            bool canAdministrate = await _coffeeMakerService.CanAdministrate( personToken );

            return Json( canAdministrate );
        }
    }
}
