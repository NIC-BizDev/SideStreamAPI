using SideStream.API.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SideStream.API.Services.IOC
{
    public interface IPointLayerProvider
    {
        MashupDataSource DataSource { get; }
        PointLayer GetPointLayerByBounds(double neLat, double neLng, double swLat, double swLng, int page = 1);
    }
}
