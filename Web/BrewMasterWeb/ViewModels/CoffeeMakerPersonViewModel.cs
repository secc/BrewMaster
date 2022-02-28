using System;
using BrewMasterWeb.Models;

namespace BrewMasterWeb.ViewModels
{
    public class CoffeeMakerPersonViewModel
    {
        public CoffeeMakerPersonViewModel()
        {
        }
        public CoffeeMakerPersonViewModel( CoffeeMaker coffeeMaker )
        {
            Id = coffeeMaker.Id;
            Name = coffeeMaker.Name;
            LastCompeteDateTime = coffeeMaker.LastCompeteDateTime;
            LastStartDateTime = coffeeMaker.LastStartDateTime;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime LastStartDateTime { get; set; }
        public DateTime LastCompeteDateTime { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
