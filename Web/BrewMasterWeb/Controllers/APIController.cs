using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrewMasterWeb.Data;
using BrewMasterWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrewMasterWeb.Controllers
{
    public class APIController : Controller
    {

        IContainerFactory _containerFactory;
        public APIController(IContainerFactory containerFactory)
        {
            _containerFactory = containerFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CoffeeMakers()
        {
            //Should make this more testable
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();
            var coffeeMakers = await coffeeMakerService.GetAllAsync("select * from c");

            return Json(coffeeMakers.ToList());
        }
    }
}
