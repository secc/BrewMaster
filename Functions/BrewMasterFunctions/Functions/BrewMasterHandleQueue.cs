using System;
using System.Threading.Tasks;
using BrewMasterFunctions.Contracts;
using BrewMasterFunctions.Data;
using BrewMasterFunctions.Model;
using BrewMasterFunctions.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BrewMasterFunctions.Functions
{
    public class BrewMasterFunctions
    {
        private readonly IContainerFactory _containerFactory;

        public BrewMasterFunctions(IContainerFactory containerFactory)
        {
            _containerFactory = containerFactory;
        }

        [FunctionName("BrewMasterHandleQueue")]
        public async Task Run(
            [ServiceBusTrigger("brewevent", Connection = "BrewMasterServiceBusConnectionString")] string message,
            ILogger log
            )
        {
            BrewMasterEvent brewMasterEvent;
            
                brewMasterEvent = JsonSerializer.Deserialize<BrewMasterEvent>( message );
           

            if (brewMasterEvent == null)
            {
                return;
            }

            switch (brewMasterEvent.BrewMasterEventType)
            {
                case BrewMasterEventType.NoEvent:
                    await HandleNoEvent(brewMasterEvent);
                    break;
                case BrewMasterEventType.BrewStart:
                    await HandleBrewStart(brewMasterEvent);
                    break;
                case BrewMasterEventType.BrewComplete:
                    await HandleBrewComplete(brewMasterEvent);
                    break;
            }
        }

        private async Task HandleBrewComplete(BrewMasterEvent brewMasterEvent)
        {
            await EnsureCoffeeMakerExists(brewMasterEvent);
            await RecordBrewEvent(brewMasterEvent);
            //await NotifyCompleteBrew(brewMasterEvent);
        }

        private async Task HandleBrewStart(BrewMasterEvent brewMasterEvent)
        {
            await EnsureCoffeeMakerExists(brewMasterEvent);
            await RecordBrewEvent(brewMasterEvent);

        }

        private async Task HandleNoEvent(BrewMasterEvent brewMasterEvent)
        {
            await EnsureCoffeeMakerExists(brewMasterEvent);
        }

        private async Task RecordBrewEvent(BrewMasterEvent brewMasterEvent)
        {
            var brewEventService = _containerFactory.GetService<BrewEvent>();
            var brewEvent = new BrewEvent
            {
                Id = Guid.NewGuid().ToString(),
                BrewMasterEventType = brewMasterEvent.BrewMasterEventType,
                CoffeeMakerId = brewMasterEvent.DeviceId,
                CompleteDateTime = brewMasterEvent.CompleteDateTime,
                StartDateTime = brewMasterEvent.StartDateTime
            };
            await brewEventService.AddAsync(brewEvent, brewEvent.Id);
        }

        private async Task EnsureCoffeeMakerExists(BrewMasterEvent brewMasterEvent)
        {
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();

            var coffeeMaker = await coffeeMakerService.GetAsync(brewMasterEvent.DeviceId);

            if (coffeeMaker == null)
            {
                coffeeMaker = new CoffeeMaker
                {
                    Id = brewMasterEvent.DeviceId,
                    LastCompeteDateTime = brewMasterEvent.CompleteDateTime,
                    LastStartDateTime = brewMasterEvent.StartDateTime
                };

                await coffeeMakerService.AddAsync(coffeeMaker, coffeeMaker.Id);
            }
            else
            {
                coffeeMaker.LastStartDateTime = brewMasterEvent.StartDateTime;
                coffeeMaker.LastCompeteDateTime = brewMasterEvent.CompleteDateTime;

                await coffeeMakerService.UpdateAsync(coffeeMaker.Id, coffeeMaker);
            }

        }

        private Task NotifyCompleteBrew(BrewMasterEvent brewMasterEvent)
        {
            //We do nothing.. yet.
            return null;
        }
    }
}