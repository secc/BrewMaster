using System;
using System.Configuration;
using BrewMasterFunctions.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(BrewMasterFunctions.Startup.Startup))]

namespace BrewMasterFunctions.Startup
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddScoped<IContainerFactory, ContainerFactory>();
           

        }
    }
}