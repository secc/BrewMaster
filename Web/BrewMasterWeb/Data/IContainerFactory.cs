using System;
namespace BrewMasterWeb.Data
{
    public interface IContainerFactory
    {
        public CosmosDbService<T> GetService<T>();
    }
}
