using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.GeoJson
{
    public class PointLayer
    {
        public PointLayer()
        {
            this.DataSource = MashupDataSource.None;
            TotalResults = 0;
            Page = -1;
            PageCount = 0;
        }
        public PointLayer(MashupDataSource dataSource):this()
        {
            this.DataSource = dataSource;
        }

        public MashupDataSource DataSource { get; set; }
        public int TotalResults { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public IList<Point> Points { get; set; }
        public int PageSize { get; set; }
    }
}