using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrewMaster.Utilities
{
    public static class ByteStringExtensions
    {
        public static byte[] CleanByteOrderMark( this byte[] bytes )
        {
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var empty = Enumerable.Empty<byte>();
            if ( bytes.Take( 3 ).SequenceEqual( bom ) )
                return bytes.Skip( 3 ).ToArray();

            return bytes;
        }
    }
}
