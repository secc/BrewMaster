using System;
using System.ComponentModel.DataAnnotations.Schema;
using BrewMasterFunctions.Attributes;
using BrewMasterFunctions.Utilities;
using Newtonsoft.Json;

namespace BrewMasterFunctions.Model
{
    [CosmosSettings("brewmasterdb", "BrewEvent")]
    public class BrewEvent
    {
        public string Id { get; set; }
        public string CoffeeMakerId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime CompleteDateTime { get; set; }
        public BrewMasterEventType BrewMasterEventType { get; set; }
    }
}
