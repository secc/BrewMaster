using System;
using BrewMasterFunctions.Utilities;

namespace BrewMasterFunctions.Contracts
{
    public class BrewMasterEvent
    {
        public string DeviceId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime CompleteDateTime { get; set; }
        public BrewMasterEventType BrewMasterEventType { get; set; }
    }
}
