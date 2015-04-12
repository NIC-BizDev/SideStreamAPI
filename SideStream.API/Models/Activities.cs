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
            var recAreas = RIDB.GetRecAreas(lat, lon, radius);
            var recAreaJObject = JArray.Parse(recAreas);
            var recAreaChildren = recAreaJObject.Children<JObject>();
            foreach (var recArea in recAreaChildren.ToList())
            {
                var id = recArea["RecAreaID"].ToString();
                var recLat = recArea["RecAreaLatitude"].ToString();
                var recLon = recArea["RecAreaLongitude"].ToString();
                AddActivitiesUnlessThere(Events, RIDB.GetRecAreaEvents(id), "EventID", recLat, recLon);
            }
            var facilities = RIDB.GetFacilities(lat, lon, radius);
            foreach (var facility in JObject.Parse(facilities).Children().ToList())
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
            var things = JArray.Parse(json);
            foreach (var thing in things.Children<JObject>().ToList())
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