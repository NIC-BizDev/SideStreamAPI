using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API
{
    [Flags]
    public enum MashupDataSource
    {
        None = 0,
        RIDB = 1,
        Flickr = 2,
        Facebook = 4,
        Twitter = 8,
        Instagram = 16,
        Picasa = 32,
        YouTube = 64,
        Yelp = 128,
        Expedia = 256
    }
}