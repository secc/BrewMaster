using System;
namespace BrewMasterFunctions.Attributes
{
    public class CosmosSettingsAttribute : Attribute
    {
        public string Database { get; set; }
        public string Table { get; set; }
        public CosmosSettingsAttribute(string database, string table)
        {
            Database = database;
            Table = table; 
        }
    }
}
