using FlickrNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideStream.API.GeoJson
{
    public class MapLayersResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MapLayersResult);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SortedDictionary<string, int> tagCount = new SortedDictionary<string, int>();
            MapLayersResult result = value as MapLayersResult;

            int remainingRecords = 0;

            writer.WriteStartObject();



            // Layers
            writer.WritePropertyName("layers");
            writer.WriteStartArray();
            foreach (PointLayer layer in result.PointLayers)
            {
                remainingRecords += (layer.TotalResults - layer.Points.Count);
                if(layer.Page > 1)
                {
                    remainingRecords -= (layer.PageSize * (layer.Page - 1));
                }

                writer.WriteStartObject();

                writer.WritePropertyName("type");
                writer.WriteValue("PointLayer");

                writer.WritePropertyName("ds");
                writer.WriteValue(layer.DataSource.ToString());

                writer.WritePropertyName("cnt");
                writer.WriteValue(layer.TotalResults);

                writer.WritePropertyName("pg");
                writer.WriteValue(layer.Page);

                writer.WritePropertyName("pgCnt");
                writer.WriteValue(layer.PageCount);

                writer.WritePropertyName("data");
                writer.WriteGeoJson(layer.Points, ref tagCount);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WritePropertyName("remainingRecords");
            writer.WriteValue(remainingRecords);

            // Tags
            writer.WritePropertyName("tags");
            writer.WriteStartArray();
            foreach (KeyValuePair<string,int> cnt in tagCount)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("tag");
                writer.WriteValue(cnt.Key);

                writer.WritePropertyName("cnt");
                writer.WriteValue(cnt.Value);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
            
        }
    }
}