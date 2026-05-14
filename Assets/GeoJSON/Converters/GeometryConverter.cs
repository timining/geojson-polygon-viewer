using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeoJSON
{
    public class GeometryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Geometry).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            string typeStr = jObject["type"].ToString();

            Geometry geometry = null;

            switch (typeStr)
            {
                case "Point":
                    geometry = new Point();
                    break;
                case "LineString":
                    geometry = new LineString();
                    break;
                case "Polygon":
                    geometry = new Polygon();
                    break;
                case "MultiPoint":
                    geometry = new MultiPoint();
                    break;
                case "MultiLineString":
                    geometry = new MultiLineString();
                    break;
                case "MultiPolygon":
                    geometry = new MultiPolygon();
                    break;
                default:
                    throw new JsonSerializationException($"Unsupported geometry type: {typeStr}");
            }

            serializer.Populate(jObject.CreateReader(), geometry);
            return geometry;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Serialization not implemented");
        }
    }
}
