using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.GeoJson
{
    public static class JsonWriterExtensions
    {
        public static void WriteGeoJson(this JsonWriter writer, IList<Point> points, ref SortedDictionary<string,int> tagCount)
        {
            // Feature Collection
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("FeatureCollection");
            writer.WritePropertyName("features");
            writer.WriteStartArray();

            foreach(Point p in points)
            {
                // Feature
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue("Feature");
                writer.WritePropertyName("id");
                writer.WriteValue(p.UniqueId);

                // Geometry
                writer.WritePropertyName("geometry");
                writer.WriteStartObject();       
                    writer.WritePropertyName("type");
                    writer.WriteValue("Point");
                    writer.WritePropertyName("coordinates");
                    writer.WriteStartArray();
                        writer.WriteValue(p.Longitude);
                        writer.WriteValue(p.Latitude);
                    writer.WriteEndArray();
                writer.WriteEndObject();

                // Properties
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                    // Data Source
                    writer.WritePropertyName("ds");
                    writer.WriteValue(p.DataSource.ToString());
                    // Title
                    writer.WritePropertyName("title");
                    writer.WriteValue(p.Title);
                    //Tags
                    writer.WritePropertyName("tags");
                    writer.WriteStartArray();
                    foreach(string tag in p.Tags)
                    {
                        writer.WriteValue(tag);

                        if (tagCount.ContainsKey(tag))
                            tagCount[tag]++;
                        else
                            tagCount.Add(tag, 1);
                    }
                    writer.WriteEndArray();
                    // More Properties
                    foreach(KeyValuePair<string,object> prop in p.Properties)
                    {
                        writer.WritePropertyName(prop.Key);
                        writer.WriteValue(prop.Value);
                    }
                writer.WriteEndObject();

                // END Feature
                writer.WriteEndObject();

            }

            // END Feature Collection
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

    }
}