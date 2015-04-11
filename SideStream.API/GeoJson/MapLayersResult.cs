using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.GeoJson
{
    public class MapLayersResult
    {

        IList<PointLayer> _pointLayers = new List<PointLayer>();
        public IList<PointLayer> PointLayers
        {
            get
            {
                return _pointLayers;
            }
        }

        public void AddLayer(PointLayer pointLayer)
        {
            this.PointLayers.Add(pointLayer);
        }
    }
}