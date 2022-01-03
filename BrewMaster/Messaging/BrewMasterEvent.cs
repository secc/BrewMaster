using System;
namespace BrewMaster.Messaging
{
    public class BrewMasterEvent
    {
        public string DeviceId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime CompleteDateTime { get; set; }
        public BrewMasterEventType BrewMasterEventType { get; set; }

    }

    public enum BrewMasterEventType
    {
        NoEvent = 0,
        BrewStart = 1,
        BrewComplete = 2
    }
}
