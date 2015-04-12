using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.Common
{
    public struct VectorLocation
    {
        public double Latitude;
        public double Longitude;
        public double Radius;
    }

    public static class Util
    {
        private static int _earthRadius = 3961; //miles
        private static double _radiusMod = .8; //From 0 to 1;

        public static VectorLocation VectorLocationFromBounds(double neLat, double neLng, double swLat, double swLng)
        {
            VectorLocation result = new VectorLocation();

            result.Latitude = (neLat + swLat) / 2;
            result.Longitude = (neLng + swLng) / 2;

            var dlat = neLat - swLat;
            var dlon = neLng - swLng;
            var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(neLat) * Math.Cos(swLat) * Math.Pow(Math.Sin(dlon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var radius = _earthRadius * c / 2 * _radiusMod; //The .9 is just to make it a bit smaller. We'll end up missing the corners a bit, but oh well.

            return result;
        }
        
    }
}