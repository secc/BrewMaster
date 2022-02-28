using System.Collections.Generic;
using BrewMasterWeb.Attributes;

namespace BrewMasterWeb.Models
{

    [CosmosSettings( "brewmasterdb", "Subscription" )]
    public class Subscription
    {
        public string Id { get; set; }
        public int PersonAliasId { get; set; }
        public string CoffeeMakerId { get; set; }
    }
}
