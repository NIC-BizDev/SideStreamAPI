using SideStream.API.GeoJson;
using SideStream.API.Services.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SideStream.API.Controllers
{
    [RoutePrefix("maps")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MapsController : ApiController
    {
        private readonly IPointLayerProvider[] pointLayerServices;

        public MapsController(IPointLayerProvider[] pointLayerServices)
        {
            this.pointLayerServices = pointLayerServices;
        }

        [HttpGet]
        [Route("layers/get", Name = "GetMapLayers")]
        public MapLayersResult GetLayers(double neLat, double neLng, double swLat, double swLng, int page = 1, [FromUri] string[] ds = null, [FromUri] string[] tags = null)
        {
            //string[] ds = null, string[] tags = null
            // ToDo: Need to add in page number and layer flags to the signature
            // might even want to add a properties collection to pass unknown get params to the providers, that would allow more flexibility later

            MapLayersResult result = new MapLayersResult();

            foreach (IPointLayerProvider pointLayerService in pointLayerServices)
            {

                if(page > 0)
                {
                    result.AddLayer(pointLayerService.GetPointLayerByBounds(neLat, neLng, swLat, swLng, page,ds,tags));
                }

            }

            return result;
        }

    }
}
