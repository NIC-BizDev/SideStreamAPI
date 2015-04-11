﻿using SideStream.API.GeoJson;
using SideStream.API.Services;
using SideStream.API.Services.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SideStream.API.Controllers
{
    [RoutePrefix("activities")]
    public class ActivitiesController : ApiController
    {
        [HttpGet]
        [Route("activities/get")]
        public string GetActivities(string lat, string lon, string radius)
        {
            return Activities.Get(lat, lon, radius);
        }
    }
}