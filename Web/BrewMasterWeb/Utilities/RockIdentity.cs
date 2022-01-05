using System;
using System.Security.Principal;

namespace BrewMasterWeb.Utilities
{
    public class RockIdentity : IIdentity
    {
        public RockIdentity(string name)
        {
            _name = name;
        }

        public string AuthenticationType => "Cookie";

        public bool IsAuthenticated => true;

        private readonly string _name;
        public string Name => _name;
    }
}
