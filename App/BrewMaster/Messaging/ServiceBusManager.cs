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
        public static async Task<bool> SendEvent(this MainPageViewModel mainPageViewModel, BrewMasterEventType brewMasterEventType)
        {
            var platform = DependencyService.Get<IDevice>();
            string deviceId = platform.GetDeviceName();

            var connectionString = Environment.GetEnvironmentVariable(MessagingConstants.BREWEVENT_CONNECTION_STRING);

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
    }
}
