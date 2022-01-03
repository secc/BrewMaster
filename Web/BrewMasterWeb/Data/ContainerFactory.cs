using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using BrewMasterWeb.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace BrewMasterWeb.Data
{
    public class ContainerFactory : IContainerFactory
    {
        private IConfiguration _configuration;

        public ContainerFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CosmosDbService<T> GetService<T>()
        {
            string url = _configuration.GetValue<string>("CosmosDbUrl");
            string key = _configuration.GetValue<string>("CosmosDbKey");
            var client = new CosmosClientBuilder(url, key)
                .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .Build();

            var modelType = typeof(T);
            CosmosSettingsAttribute  settingsAttribute =
                (CosmosSettingsAttribute)Attribute.GetCustomAttribute(modelType, typeof(CosmosSettingsAttribute));

            if (settingsAttribute == null)
            {
                return default;
            }

            var service = new CosmosDbService<T>(client, settingsAttribute.Database, settingsAttribute.Table);
            return service;
        }
    }
}
