using SideStream.API.Common;
using SideStream.API.GeoJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using YelpSharp;
using YelpSharp.Data;
using YelpSharp.Data.Options;

namespace SideStream.API.Services
{
    public class YelpService : IOC.IPointLayerProvider
    {
        private static int pageSize = 20;

        public MashupDataSource DataSource
        {
            get { return MashupDataSource.Yelp; }
        }

        public GeoJson.PointLayer GetPointLayerByBounds(double neLat, double neLng, double swLat, double swLng, int page = 1, string[] ds = null, string[] tags = null)
        {

            //Consumer Key:	kVvCwX0ocRhDHbA9LfdLrQ
            //Consumer Secret:	RCJdVyZ5R3pVoc3hzyXIy-0ILQ0
            //Token:	ZgOH_PB5SkKIOIIeI3_9vYRE5ZNTZiIn
            //Token Secret:	rSn8bgTH44an2XYJTgqQQqcDubs





            PointLayer layer = new PointLayer(this.DataSource);

            var options = new Options()
            {
                AccessToken = "ZgOH_PB5SkKIOIIeI3_9vYRE5ZNTZiIn",
                AccessTokenSecret = "rSn8bgTH44an2XYJTgqQQqcDubs",
                ConsumerKey = "kVvCwX0ocRhDHbA9LfdLrQ",
                ConsumerSecret = "RCJdVyZ5R3pVoc3hzyXIy-0ILQ0"
            };

            Yelp yelp = new Yelp(options);

            SearchOptions searchOptions = new SearchOptions()
            {
                LocationOptions = new BoundOptions()
                {
                    sw_latitude = swLat,
                    sw_longitude = swLng,
                    ne_latitude = neLat,
                    ne_longitude = neLng
                }
            };

            //VectorLocation vLocation = Util.VectorLocationFromBounds(neLat, neLng, swLat, swLng);

            //LocationOptions location = new LocationOptions();
            //CoordinateOptions coords = new CoordinateOptions();
            //coords.latitude = vLocation.Latitude;
            //coords.longitude = vLocation.Longitude;
            //location.coordinates = coords;
            //// location.location = coords;
            

            //searchOptions.LocationOptions = location;

            GeneralOptions generalOptions = new GeneralOptions();
            //generalOptions.radius_filter = vLocation.Radius;
            generalOptions.limit = pageSize;
            

            if(page > 1)
            {
                generalOptions.offset = (page - 1) * pageSize;
            }

            searchOptions.GeneralOptions = generalOptions;

            SearchResults results = yelp.Search(searchOptions).Result;
            
            
            layer.TotalResults = results.total;
            layer.Page = page;
            layer.PageCount = layer.TotalResults % pageSize;
            layer.Points = PointsFromBusinessList(results.businesses);
            layer.PageSize = pageSize;

            return layer;

        }

        private IList<Point> PointsFromBusinessList(IList<Business> businesses)
        {
            IList<Point> points = new List<Point>();

            foreach (Business b in businesses)
            {
                Point point = new Point(this.DataSource);
                point.Id = b.id;
                point.Title = b.name;
                point.Latitude = b.location.coordinate.latitude;
                point.Longitude = b.location.coordinate.longitude;

                point.Properties.Add("desc", b.snippet_text);
                // point.Properties.Add("owner", b.name);
                // point.Properties.Add("taken", p.DateTaken.ToString("MMM d, yyyy @h:mtt"));
                // point.Properties.Add("likes", b.);
                point.Properties.Add("thumbnail", b.snippet_image_url);
                //point.Properties.Add("thumbnail", p.SquareThumbnailUrl);

                if (b.image_url != null)
                    point.Properties.Add("image", b.image_url.Replace("ms.","o."));
                else
                    point.Properties.Add("image", "images/default.jpg");

                point.Properties.Add("rating", b.rating);
                point.Properties.Add("ratingImgUrl", b.rating_img_url);
                point.Properties.Add("reviewCount",b.review_count);
                point.Properties.Add("phone", b.phone);

                IList<string> tags = new List<string>();
                if (b.categories != null)
                {
                    foreach (var c in b.categories)
                    {
                        tags.Add(c[0].ToString().Replace(" ", "").ToLower());
                    }
                }

                point.AddTags(tags);

                points.Add(point);
            }

            return points;
        }
    }
}