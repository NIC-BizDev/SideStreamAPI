using SideStream.API.Models;
using SideStream.API.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.Services
{
    public class ActivitiesService : IOC.IPointLayerProvider
    {
        private int _earthRadius = 3961; //miles
        private double _radiusMod = .8; //From 0 to 1;

        public MashupDataSource DataSource
        {
            get { return MashupDataSource.RIDB; }
        }

        public PointLayer GetPointLayerByBounds(double neLat, double neLng, double swLat, double swLng, int page = 1, string[] ds = null, string[] tags = null)
        {
            var latCenter = (neLat + swLat) / 2;
            var lonCenter = (neLng + swLng) / 2;

            var dlat = neLat - swLat;
            var dlon = neLng - swLng;
            var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(neLat) * Math.Cos(swLat) * Math.Pow(Math.Sin(dlon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var radius = _earthRadius * c / 2 * _radiusMod; //The .9 is just to make it a bit smaller. We'll end up missing the corners a bit, but oh well.
        
            var activities = new Activities(latCenter.ToString(), lonCenter.ToString(), radius.ToString());

            var retval = new PointLayer()
            {
                DataSource = this.DataSource,
                Page = 1,
                PageCount = 1,
                Points = new List<Point>(),
                TotalResults = 0,
            };

            foreach (var activity in activities.ToList())
            {
                var newPoint = new Point()
                {
                    DataSource = this.DataSource,
                    Latitude = Convert.ToDouble(activity["lat"].ToString()),
                    Longitude = Convert.ToDouble(activity["lon"].ToString()),
                    Properties = new Dictionary<string,object>(),
                };
                newPoint.Properties.Add("details", activity);
                newPoint.AddTag("Activity");
                var eventId = TryToConvertToInt(activity["EventID"]);
                var tourId = TryToConvertToInt(activity["TourID"]);
                if (eventId.HasValue)
                {
                    newPoint.Title = activity["EventName"].ToString();
                    newPoint.Id = eventId.Value.ToString();
                }
                else if (tourId.HasValue)
                {
                    newPoint.Title = activity["TourName"].ToString();
                    newPoint.Id = tourId.Value.ToString();
                }
                else
                {
                    newPoint.Title = "Unknown Activity";
                    newPoint.Id = retval.Points.Count.ToString();
                }

                retval.Points.Add(newPoint);                
            }

            retval.TotalResults = retval.Points.Count;

            return retval;
        }

        private int? TryToConvertToInt(object value)
        {
            int? retval = null;
            if (value != null)
            {
                var str = value.ToString();
                if (!String.IsNullOrWhiteSpace(str))
                {
                    try 
                    {
                        retval = Convert.ToInt32(str);
                    }
                    catch
                    {
                        retval = null;
                    }
                }
            }
            return retval;
        }
    }
}