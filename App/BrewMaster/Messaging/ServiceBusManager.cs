using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BrewMaster.Interfaces;
using BrewMaster.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BrewMaster.Messaging
{
    public static class ServiceBusManager
    {
        private static string _connectionString;


        public static async Task<bool> SendEvent( this MainPageViewModel mainPageViewModel, BrewMasterEventType brewMasterEventType )
        {
            var platform = DependencyService.Get<IDevice>();
            string deviceId = platform.GetDeviceName();

            if ( await GetConnectionString() is null )
            {
                return false;
            }

            var brewEvent = new BrewMasterEvent
            {
                BrewMasterEventType = brewMasterEventType,
                CompleteDateTime = mainPageViewModel.BrewDateTime,
                StartDateTime = mainPageViewModel.BrewStartDateTime,
                DeviceId = deviceId
            };

            var payload = JsonConvert.SerializeObject( brewEvent );

            var client = new ServiceBusClient( await GetConnectionString() );
            var sender = client.CreateSender( "brewevent" );
#if DEBUG
            await sender.SendMessageAsync( new ServiceBusMessage( payload ) );
#else
            try {
                await sender.SendMessageAsync( new ServiceBusMessage( payload ) );
            } catch { return false;}
            
#endif
            return true;
        }

        private async static Task<string> GetConnectionString()
        {
            if ( _connectionString is null )
            {
                var fileManager = DependencyService.Get<IFileStorage>();
                var configFile = await fileManager.ReadAsString( "config.json" );
                var config = JsonConvert.DeserializeObject<Configuration>( configFile );
                _connectionString = config.ServiceBusConnectionString;
            }
            return _connectionString;
        }

    }
}
