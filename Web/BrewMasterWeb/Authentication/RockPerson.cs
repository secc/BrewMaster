using System;
namespace BrewMasterWeb.Authentication
{
    public class RockPerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int PrimaryAliasId { get; set; }
    }
}
