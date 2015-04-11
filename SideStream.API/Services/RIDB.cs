using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace SideStream.API.Services
{
    public static class RIDB
    {
        private static string _ridbBaseurl = ConfigurationManager.AppSettings["RIDBBaseUrl"];
        private static string _ridbVersion = ConfigurationManager.AppSettings["RIDBVersion"];
        private static string _ridbApiKey = ConfigurationManager.AppSettings["RIDBkey"];

        public static string GetRecAreas(string lat, string lon, string radius)
        {
            var parameters = new Dictionary<string, string>() {
                { "latitude", lat },
                { "longitude", lon },
                { "radius", radius }
            };
            return CallRIDB(parameters, "recareas");
        }

        public static string GetRecAreaActivities(string recAreaId)
        {
            return CallRIDB("recareas", recAreaId, "activities");
        }

        public static string GetRecAreaEvents(string recAreaId)
        {
            return CallRIDB("recareas", recAreaId, "events");
        }

        public static string GetFacilities(string lat, string lon, string radius)
        {
            var parameters = new Dictionary<string, string>() {
                { "latitude", lat },
                { "longitude", lon },
                { "radius", radius }
            };
            return CallRIDB(parameters, "facilities");
        }

        public static string GetFacilityActivities(string facilityId)
        {
            return CallRIDB("facilities", facilityId, "activities");
        }

        public static string GetFacilityEvents(string facilityId)
        {
            return CallRIDB("facilities", facilityId, "events");
        }

        public static string GetFacilityTours(string facilityId)
        {
            return CallRIDB("facilities", facilityId, "tours");
        }

        public static string CallRIDB(params string[] pieces)
        {
            return CallRIDB(pieces);
        }
        public static string CallRIDB(Dictionary<string, string> parameters, params string[] pieces)
        {
            var url = GetRidbUrl(pieces);
            var client = new WebClient();
            client.Headers.Add("apikey", _ridbApiKey);
            return client.DownloadString(url);
        }

        public static string GetRidbUrl(params string[] pieces)
        {
            return String.Format("https://{0}/{1}/{2}", _ridbBaseurl, _ridbVersion, String.Join("/", pieces));
        }

    }
}