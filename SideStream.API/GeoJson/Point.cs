using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.GeoJson
{
    public class Point
    {

        public Point()
        {
            this.Properties = new Dictionary<string, object>();
            this.DataSource = MashupDataSource.None;
        } 

        public Point(MashupDataSource dataSource):this()
        {
            this.DataSource = dataSource;
        }

        public MashupDataSource DataSource { get; set; }
        public string Id { get; set; }
        public string UniqueId
        {
            get
            {
                switch(this.DataSource)
                {
                    case MashupDataSource.Flickr:
                        return "F" + this.Id;
                    default:
                        return "U" + this.Id;
                }
            }
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Title { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        SortedSet<string> _tags = new SortedSet<string>();
        public SortedSet<string> Tags
        {
            get
            {
                return _tags;
            }
        }
        public void AddTag(string tag)
        {
            Tags.Add(tag);
        }
        public void AddTags(string tagsCsv)
        {
            foreach (string tag in tagsCsv.Split(','))
            {
                AddTag(tag);
            }
        }

        public void AddTags(System.Collections.ObjectModel.Collection<string> tagCollection)
        {
            foreach (string tag in tagCollection)
                this.AddTag(tag);
        }
    }
}