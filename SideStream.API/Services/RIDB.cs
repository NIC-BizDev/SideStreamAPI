using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            return CallRIDB(new Dictionary<string, string>(), pieces);
        }
        public static string CallRIDB(IDictionary<string, string> parameters, params string[] pieces)
        {
            var resultObject = GetRIDBPage(parameters, pieces);
            var retvalParts = new List<JToken>();
            retvalParts.AddRange(resultObject["RECDATA"].Children());
            if (!parameters.ContainsKey("offset"))
            {
                var totalCount = TryToConvert.ToInt(resultObject["METADATA"]["RESULTS"]["TOTAL_COUNT"]);
                var currentCount = TryToConvert.ToInt(resultObject["METADATA"]["RESULTS"]["CURRENT_COUNT"]);
                if (currentCount < totalCount)
                {
                    var limit = TryToConvert.ToInt(resultObject["METADATA"]["RESULTS"]["LIMIT"]) ?? 50;
                    var pageCount = totalCount / limit + (totalCount % limit > 0 ? 1 : 0);
                    //offset is zero based, and we've already got the first one.
                    for (int i = 1; i < pageCount; i++)
                    {
                        parameters["offset"] = i.ToString();
                        var pageResultObject = GetRIDBPage(parameters, pieces);
                        retvalParts.AddRange(pageResultObject["RECDATA"].Children());
                    }
                }
            }
            return JsonConvert.SerializeObject(retvalParts);
        }

        public static JObject GetRIDBPage(IDictionary<string, string> parameters, params string[] pieces)
        {
            var url = GetRidbUrl(pieces, parameters);
            var client = new WebClient();
            client.Headers.Add("apikey", _ridbApiKey);
            var result = client.DownloadString(url);
            return JObject.Parse(result);
        }

        public static string addParamsToUrl(string baseUrl, IDictionary<string, string> parameters)
        {
            return addParamStringToUrl(baseUrl, createParamsString(parameters));
        }

        public static string GetRidbUrl(string[] pieces, IDictionary<string, string> parameters)
        {
            return addParamStringToUrl(GetRidbBaseUrl(pieces), createParamsString(parameters));
        }

        public static string addParamStringToUrl(string baseUrl, string paramString)
        {
            return baseUrl + (String.IsNullOrEmpty(paramString) ? "" : "?" + paramString);
        }

        public static string GetRidbBaseUrl(params string[] pieces)
        {
            return String.Format("https://{0}/{1}/{2}", _ridbBaseurl, _ridbVersion, String.Join("/", pieces));
        }

        public static string createParamsString(IDictionary<string, string> parameters)
        {
            var parts = new List<string>();
            foreach (var key in parameters.Keys)
            {
                parts.Add(key + "=" + parameters[key]);
            }
            return String.Join("&", parts);
        }

    }
}