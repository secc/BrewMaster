using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrewMasterWeb.Data;
using BrewMasterWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrewMasterWeb.Controllers
{
    public class ComponentsController : Controller
    {

        public IActionResult CoffeeMakers()
        {
            return View();
        }
    }
}
