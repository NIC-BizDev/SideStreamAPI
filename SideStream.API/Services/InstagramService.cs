using InstagramCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.Services
{
    public class InstagramService : IOC.IPointLayerProvider
    {
        public MashupDataSource DataSource
        {
            get { return MashupDataSource.Instagram; }
        }

        public GeoJson.PointLayer GetPointLayerByBounds(double neLat, double neLng, double swLat, double swLng, int page = 1, string[] ds = null, string[] tags = null)
        {
            


            string clientId = "db1265f56e574479b72be26cb13aae9b";
            string clientSecret = "1515c3f447094eafa18631f06d16d204";
            string accessToken = "1515c3f447094eafa18631f06d16d204";

            InstagramClient client = new InstagramClient(clientId, "1515c3f447094eafa18631f06d16d204","FRACK - U need to make the user sign in");

            //var popularMedia = await client.MediaEndpoints.GetPopularMediaAsync();

            ////I use Json.NET for parsing the result
            //var popularMediaJson = JsonConvert.DeserializeObject(popularMedia);

            ////You can deserialize json result to one of the models in InstagramCSharp or to your custom model
            ////var popularMediaJson = JsonConvert.DeserializeObject<MediaFeed>(popularMedia);

            //return popularMediaJson;
            return null;


        }
    }
}