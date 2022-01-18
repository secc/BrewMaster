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

            var connectionString = await GetConnectionString();
            if ( connectionString is null )
            {
                await App.Current.MainPage.DisplayAlert( "Connection String Error", "Could not load connection string", "ok" );
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
            await App.Current.MainPage.DisplayAlert( "connectionString", connectionString, "ok" );
            var client = new ServiceBusClient( connectionString );
            var sender = client.CreateSender( "brewevent" );
#if DEBUG
            sender.SendMessageAsync( new ServiceBusMessage( payload ) );
#else
            try {
                sender.SendMessageAsync( new ServiceBusMessage( payload ) );
            catch ( Exception e )
            {
                await App.Current.MainPage.DisplayAlert( "error", e.Message, "ok" );
            }
            
#endif
            return true;
        }

        private async static Task<string> GetConnectionString()
        {
            try
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
            catch ( Exception e )
            {
                await App.Current.MainPage.DisplayAlert( "error", e.Message, "ok" );
            }
            return null;
        }

    }
}
