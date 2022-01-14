using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrewMaster.Interfaces
{
    public interface IFileStorage
    {
        Task<byte[]> ReadAsBytes( string filename );

        Task<string> ReadAsString( string filename );
    }
}
