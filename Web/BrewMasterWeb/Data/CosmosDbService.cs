using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrewMasterWeb.Models;
using Microsoft.Azure.Cosmos;

namespace BrewMasterWeb.Data
{

    public class CosmosDbService<T>
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName )
        {
            this._container = dbClient.GetContainer( databaseName, containerName );
        }

        public IQueryable<T> Queryable()
        {
            return _container.GetItemLinqQueryable<T>();
        }

        public async Task AddAsync( T item, string id )
        {
            await this._container.CreateItemAsync<T>( item, new PartitionKey( id ) );
        }

        public async Task DeleteAsync( string id )
        {
            await this._container.DeleteItemAsync<T>( id, new PartitionKey( id ) );
        }

        public async Task<T> GetAsync( string id )
        {
            try
            {
                ItemResponse<T> response = await this._container.ReadItemAsync<T>( id, new PartitionKey( id ) );
                return response.Resource;
            }
            catch ( CosmosException ex ) when ( ex.StatusCode == System.Net.HttpStatusCode.NotFound )
            {
                return default;
            }

        }

        public async Task<IEnumerable<T>> GetAllAsync( string queryString )
        {
            var query = this._container.GetItemQueryIterator<T>( new QueryDefinition( queryString ) );
            List<T> results = new List<T>();
            while ( query.HasMoreResults )
            {
                var response = await query.ReadNextAsync();

                results.AddRange( response.ToList() );
            }

            return results;
        }

        public async Task UpdateAsync( string id, T item )
        {
            await this._container.UpsertItemAsync<T>( item, new PartitionKey( id ) );
        }
    }
}
