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
        private static string connectionString { get
            {
                if ( string.IsNullOrWhiteSpace( _connectionString) ){
                    _connectionString = GetConnectionString();
                }
                return _connectionString;
            }
        }

       

        public static async Task<bool> SendEvent(this MainPageViewModel mainPageViewModel, BrewMasterEventType brewMasterEventType)
        {
            var platform = DependencyService.Get<IDevice>();
            string deviceId = platform.GetDeviceName();

            if (connectionString is null)
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

            var payload = JsonConvert.SerializeObject(brewEvent);

            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender("brewevent");
#if DEBUG
            await sender.SendMessageAsync(new ServiceBusMessage(payload));
#else
            try {
                await sender.SendMessageAsync( new ServiceBusMessage( payload ) );
            } catch { return false;}
            
#endif
            return true;
        }

        private static string GetConnectionString()
        {
            //In testing add the connection string to the iOS environment variables. 
            //For use the build script to replace the value

#if DEBUG
            return Environment.GetEnvironmentVariable( MessagingConstants.BREWEVENT_CONNECTION_STRING );
#else
            return MessagingConstants.BREWEVENT_CONNECTION_STRING;
#endif
        }

    }
}
