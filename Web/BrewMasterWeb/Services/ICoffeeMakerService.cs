using System.Collections.Generic;
using System.Threading.Tasks;
using BrewMasterWeb.Models;
using BrewMasterWeb.ViewModels;

namespace BrewMasterWeb.Services
{
    public interface ICoffeeMakerService
    {
        Task<List<CoffeeMaker>> All();
        Task<List<CoffeeMakerPersonViewModel>> ForPerson( string personToken );
        Task Subscribe( CoffeeMakerSubscribeViewModel makerSubscribe );
        Task Unsubscribe( CoffeeMakerSubscribeViewModel makerSubscribe );

        Task UpdateName( CoffeeMakerNameUpdateViewModel nameUpdateViewModel );
        Task<bool> CanAdministrate( string personToken );
        Task ToggleActive( CoffeeMakerToggleActiveViewModel toggleActiveViewModel );
    }
}