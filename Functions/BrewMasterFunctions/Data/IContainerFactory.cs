using System;
namespace BrewMasterFunctions.Data
{
    public interface IContainerFactory
    {
        public CosmosDbService<T> GetService<T>();
    }
}
