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
                //newPoint.Properties.Add("details", activity);
                newPoint.AddTag("Activity");
                var eventId = TryToConvert.ToInt(activity["EventID"]);
                var tourId = TryToConvert.ToInt(activity["TourID"]);
                if (eventId.HasValue)
                {
                    newPoint.Id = "E" + eventId.Value.ToString();
                    newPoint.Title = activity["EventName"].ToString();

                    newPoint.AddTag("Event");

                    newPoint.Properties.Add("Type", "Event");
                    var startDate = activity["EventStartDate"].ToString();
                    var endDate = activity["EventEndDate"].ToString();
                    if (startDate == endDate)
                    {
                        newPoint.Properties.Add("Start Date", startDate);
                    }
                    else
                    {
                        newPoint.Properties.Add("Start Date", startDate);
                        newPoint.Properties.Add("End Date", endDate);
                    }
                    newPoint.Properties.Add("Description", activity["EventDescription"].ToString());
                }
                else if (tourId.HasValue)
                {
                    newPoint.Id = "T" + tourId.Value.ToString();
                    newPoint.Title = activity["TourName"].ToString();

                    newPoint.AddTag("Tour");

                    newPoint.Properties.Add("Type", "Tour");
                    newPoint.Properties.Add("Description", activity["TourDescription"].ToString());
                    newPoint.Properties.Add("Duration", activity["TourDuration"].ToString());
                    newPoint.Properties.Add("TourType", activity["TourType"].ToString());
                    foreach (var attr in activity["ATTRIBUTES"].Children().ToList())
                    {
                        newPoint.Properties.Add(attr["AttributeName"].ToString(), attr["AttributeValue"].ToString());
                    }
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

    }
}