using System;
using BrewMasterWeb.Models;
using Humanizer;

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
            LastBrewTime = ( DateTime.Now - coffeeMaker.LastCompeteDateTime ).Humanize();
            IsActive = coffeeMaker.IsHidden != true;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastStartDateTime { get; set; }
        public DateTime LastCompeteDateTime { get; set; }
        public string LastBrewTime { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
