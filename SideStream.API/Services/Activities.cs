using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.Services
{
    public static class Activities
    {
        public static string Get(string lat, string lon, string radius)
        {
            return GetActivities(lat, lon, radius);
        }
        public static string GetActivities(string lat, string lon, string radius)
        {
            var events = new Dictionary<string, JToken>();
            var activities = new Dictionary<string, JToken>();
            var tours = new Dictionary<string, JToken>();

            foreach (var recAreaId in JObject.Parse(RIDB.GetRecAreas(lat, lon, radius)).Children()["RecAreaID"].ToList().Select(j => j.ToString()))
            {
                AddItemsUnlessThere(events,     RIDB.GetRecAreaEvents(recAreaId),     "EventID"   );
                AddItemsUnlessThere(activities, RIDB.GetRecAreaActivities(recAreaId), "ActivityID");
            }
            foreach (var facilityId in JObject.Parse(RIDB.GetFacilities(lat, lon, radius)).Children()["RecAreaID"].ToList().Select(j => j.ToString()))
            {
                AddItemsUnlessThere(events,     RIDB.GetFacilityEvents(facilityId),     "EventID");
                AddItemsUnlessThere(activities, RIDB.GetFacilityActivities(facilityId), "ActivityID");
                AddItemsUnlessThere(tours,      RIDB.GetFacilityTours(facilityId),      "TourID");
            }

            var retval = new
            {
                Events = events.Values.Select(e => e as JObject).ToList(),
                Activities = activities.Values.Select(a => a as JObject).ToList(),
                Tours = tours.Values.Select(t => t as JObject).ToList(),
            };
            return JsonConvert.SerializeObject(retval, Formatting.Indented);
        }

        private static void AddItemsUnlessThere(Dictionary<string, JToken> dict, string json, string key) 
        {
            foreach (var thing in JObject.Parse(json).Children().ToList())
            {
                AddUnlessThere(dict, thing[key].ToString(), thing);
            }
        }

        private static void AddUnlessThere(Dictionary<string, JToken> dict, string key, JToken value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }
    }
}