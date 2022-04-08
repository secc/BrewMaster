using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BrewMasterWeb.Data;
using BrewMasterWeb.Models;
using BrewMasterWeb.ViewModels;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using SECC.Rock.Authentication;
using SECC.Rock.RockInterface;

namespace BrewMasterWeb.Services
{
    public class CoffeeMakerService : ICoffeeMakerService
    {
        readonly private IContainerFactory _containerFactory;
        readonly private IRockClient _rockClient;

        public CoffeeMakerService( IContainerFactory containerFactory, IRockClient rockClient )
        {
            _containerFactory = containerFactory;
            _rockClient = rockClient;
        }
        public async Task<List<CoffeeMaker>> All()
        {
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();
            var coffeeMakers = await coffeeMakerService.GetAllAsync( "select * from c" );
            return coffeeMakers.ToList();
        }

        public Task<bool> CanAdministrate( string personToken )
        {
            throw new NotImplementedException();
        }

        public async Task<List<CoffeeMakerPersonViewModel>> ForPerson( string personToken )
        {
            List<int> personAliasIds = await GetAliasIdsForPerson( personToken );
            var subscriptionService = _containerFactory.GetService<Subscription>();
            var subscriptions = await subscriptionService
                .GetAllAsync( $"select * from c where c.personAliasId in ({string.Join( ',', personAliasIds )}) " );

            var subscribedCoffeeMakerIds = subscriptions.Select( s => s.CoffeeMakerId ).ToList();

            var coffeeMakerViewModels = ( await All() )
                .Select( c => new CoffeeMakerPersonViewModel( c ) )
                .ToList();

            foreach ( var coffeeMaker in coffeeMakerViewModels )
            {
                coffeeMaker.IsSubscribed = subscribedCoffeeMakerIds.Contains( coffeeMaker.Id );
            }

            return coffeeMakerViewModels;
        }

        public async Task Subscribe( CoffeeMakerSubscribeViewModel makerSubscribe )
        {
            List<int> personAliasIds = await GetAliasIdsForPerson( makerSubscribe.PersonToken );

            var subscriptionService = _containerFactory.GetService<Subscription>();

            var feed = subscriptionService.Queryable()
                .Where( s => personAliasIds.Contains( s.PersonAliasId )
                     && s.CoffeeMakerId == makerSubscribe.CoffeeMakerId )
                .ToFeedIterator();

            var subscriptions = new List<Subscription>();

            while ( true )
            {
                var result = await feed.ReadNextAsync();
                if ( result.Count == 0 )
                {
                    break;
                }
                subscriptions.AddRange( result.Resource );
            }

            var subscribedCoffeeMakerIds = subscriptions.Select( s => s.CoffeeMakerId );

            if ( subscribedCoffeeMakerIds.Any() )
            {
                return;
            }

            var primaryAliasId = personAliasIds.LastOrDefault();
            var subscription = new Subscription
            {
                Id = Guid.NewGuid().ToString(),
                CoffeeMakerId = makerSubscribe.CoffeeMakerId,
                PersonAliasId = primaryAliasId
            };

            await subscriptionService.AddAsync( subscription, subscription.Id );
        }

        public async Task ToggleActive( CoffeeMakerToggleActiveViewModel toggleActiveViewModel )
        {
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();
            var coffeeMaker = await coffeeMakerService.GetAsync( toggleActiveViewModel.Id );
            coffeeMaker.IsHidden = !toggleActiveViewModel.IsActive;

            await coffeeMakerService.UpdateAsync( coffeeMaker.Id, coffeeMaker );
        }

        public async Task Unsubscribe( CoffeeMakerSubscribeViewModel makerSubscribe )
        {
            List<int> personAliasIds = await GetAliasIdsForPerson( makerSubscribe.PersonToken );

            var subscriptionService = _containerFactory.GetService<Subscription>();

            var feed = subscriptionService.Queryable()
                .Where( s => personAliasIds.Contains( s.PersonAliasId )
                     && s.CoffeeMakerId == makerSubscribe.CoffeeMakerId )
                .ToFeedIterator();

            while ( true )
            {
                var result = await feed.ReadNextAsync();
                if ( result.Count == 0 )
                {
                    break;
                }
                foreach ( var subscription in result.Resource )
                {
                    await subscriptionService.DeleteAsync( subscription.Id );
                }
            }
        }

        public async Task UpdateName( CoffeeMakerNameUpdateViewModel nameUpdateViewModel )
        {
            var coffeeMakerService = _containerFactory.GetService<CoffeeMaker>();
            var coffeeMaker = await coffeeMakerService.GetAsync( nameUpdateViewModel.Id );
            coffeeMaker.Name = nameUpdateViewModel.Name;

            await coffeeMakerService.UpdateAsync( coffeeMaker.Id, coffeeMaker );
        }

        private async Task<List<int>> GetAliasIdsForPerson( string personToken )
        {
            var person = await _rockClient.GetPersonByToken( personToken );
            return person?.AliasIds;

        }
    }
}
