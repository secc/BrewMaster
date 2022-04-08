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

        public async Task<IActionResult> CoffeeMakersForPerson( string personToken )
        {
            List<CoffeeMakerPersonViewModel> coffeeMakers = await _coffeeMakerService.ForPerson( personToken );

            var isAdmin = await _rockAuthService.CanAdministrate( personToken );

            if ( !isAdmin )
            {
                coffeeMakers = coffeeMakers.Where( c => c.IsActive == true ).ToList();
            }

            var output = new Dictionary<string, object>
            {
                { "coffeeMakers", coffeeMakers },
                {"isAdmin", isAdmin }
            };

            return Json( output );
        }

        [HttpPost]
        public async Task<IActionResult> UpdateName( [FromForm] CoffeeMakerNameUpdateViewModel nameUpdateViewModel )
        {
            if ( await _rockAuthService.CanAdministrate( nameUpdateViewModel.PersonToken ) )
            {
                await _coffeeMakerService.UpdateName( nameUpdateViewModel );
            }
            else
            {
                return Unauthorized();
            }


            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCoffeeMaker( [FromForm] CoffeeMakerToggleActiveViewModel toggleActiveViewModel )
        {
            if ( await _rockAuthService.CanAdministrate( toggleActiveViewModel.PersonToken ) )
            {
                await _coffeeMakerService.ToggleActive( toggleActiveViewModel );
            }
            else
            {
                return Unauthorized();
            }


            return NoContent();
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
