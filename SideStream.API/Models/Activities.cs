using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SideStream.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.Models
{
    public class Activities
    {
        public Dictionary<string, JToken> Events { get; set; }
        public Dictionary<string, JToken> Tours { get; set; }

        public Activities(string lat, string lon, string radius)
        {
            LoadActivitiesFromRIDB(lat, lon, radius);
        }

        public string GetJson()
        {
            var retval = new
            {
                events = Events,
                tours = Tours,
            };
            return JsonConvert.SerializeObject(retval, Formatting.Indented);
        }

        public IList<JToken> ToList()
        {
            var retval = new List<JToken>();
            retval.AddRange(Events.Values);
            retval.AddRange(Tours.Values);
            return retval;
        }

        private void LoadActivitiesFromRIDB(string lat, string lon, string radius)
        {
            foreach (var recArea in JObject.Parse(RIDB.GetRecAreas(lat, lon, radius)).Children().ToList())
            {
                var id = recArea["RecAreaID"].ToString();
                var recLat = recArea["RecAreaLatitude"].ToString();
                var recLon = recArea["RecAreaLongitude"].ToString();
                AddActivitiesUnlessThere(Events, RIDB.GetRecAreaEvents(id), "EventID", recLat, recLon);
            }
            foreach (var facility in JObject.Parse(RIDB.GetFacilities(lat, lon, radius)).Children().ToList())
            {
                var id = facility["FacilityID"].ToString();
                var facLat = facility["FacilityLatitude"].ToString();
                var facLon = facility["FacilityLongitude"].ToString();
                AddActivitiesUnlessThere(Events, RIDB.GetFacilityEvents(id), "EventID", facLat, facLon);
                AddActivitiesUnlessThere(Tours, RIDB.GetFacilityTours(id), "TourID", facLat, facLon);
            }
        }

        private static void AddActivitiesUnlessThere(Dictionary<string, JToken> dict, string json, string key, string lat, string lon) 
        {
            foreach (var thing in JObject.Parse(json).Children().ToList())
            {
                if (!dict.ContainsKey(key))
                {
                    thing["lat"] = lat;
                    thing["lon"] = lon;
                    dict.Add(thing[key].ToString(), thing);
                }
            }
        }
    }
}