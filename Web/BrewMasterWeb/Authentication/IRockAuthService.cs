using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BrewMasterWeb.Authentication
{
    public interface IRockAuthService
    {
        IActionResult RedirectToIdP();
        Task<bool> Authenticate(HttpContext httpContext);
    }
}
