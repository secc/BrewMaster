using System;
using BrewMasterFunctions.Attributes;
using Newtonsoft.Json;

namespace BrewMasterFunctions.Model
{
    [CosmosSettings("brewmasterdb", "CoffeeMaker")]
    public class CoffeeMaker
    {
        public string Id { get; set; }
        public string Name { get; set; } = "New Coffee Maker";
        public DateTime LastStartDateTime { get; set; }
        public DateTime LastCompeteDateTime { get; set; }
    }
}
