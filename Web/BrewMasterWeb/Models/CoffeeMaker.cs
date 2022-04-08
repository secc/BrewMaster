using System;
using BrewMasterWeb.Attributes;

namespace BrewMasterWeb.Models
{
    [CosmosSettings("brewmasterdb", "CoffeeMaker")]
    public class CoffeeMaker
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime LastStartDateTime { get; set; }
        public DateTime LastCompeteDateTime { get; set; }
        public bool? IsHidden { get; set; }
    }
}
