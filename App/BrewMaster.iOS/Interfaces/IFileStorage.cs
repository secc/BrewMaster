using System.IO;
using System.Threading.Tasks;
using BrewMaster.Interfaces;
using BrewMaster.iOS.Interfaces;
using BrewMaster.Utilities;

[assembly: Xamarin.Forms.Dependency( typeof( FileStorage ) )]
namespace BrewMaster.iOS.Interfaces
{
    public class FileStorage : IFileStorage
    {
        public Task<byte[]> ReadAsBytes( string filename )
        {
            var data = File.ReadAllBytes( filename );

            if ( data != null )
                data = data.CleanByteOrderMark();

            return Task.FromResult( data );
        }

        public async Task<string> ReadAsString( string filename )
        {
            var data = await ReadAsBytes( filename );

            if ( data == null )
                return string.Empty;

            return System.Text.Encoding.UTF8.GetString( data );
        }


    }
}