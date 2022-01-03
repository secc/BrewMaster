using System;
using BrewMaster.Interfaces;
using BrewMaster.iOS.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(Device_iOS))]
namespace BrewMaster.iOS.Interfaces
{
    public class Device_iOS : IDevice
    {
        public string GetDeviceName()
        {
            return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
    }
}