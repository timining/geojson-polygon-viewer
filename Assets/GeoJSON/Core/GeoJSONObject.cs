using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GeoJSON
{
    public enum GeoJSONObjectType
    {
        Point,
        MultiPoint,
        LineString,
        MultiLineString,
        Polygon,
        MultiPolygon,
        GeometryCollection,
        Feature,
        FeatureCollection
    }

    public abstract class GeoJSONObject
    {
        public GeoJSONObjectType type;

        public static GeoJSONObject Parse(string json)
        {
            string sanitizedJson = SanitizeInvalidJson(json);
            JObject jObject = JObject.Parse(sanitizedJson);
            string typeStr = jObject["type"].ToString();

            switch (typeStr)
            {
                case "Point":
                    return JsonConvert.DeserializeObject<Point>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "MultiPoint":
                    return JsonConvert.DeserializeObject<MultiPoint>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "LineString":
                    return JsonConvert.DeserializeObject<LineString>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "MultiLineString":
                    return JsonConvert.DeserializeObject<MultiLineString>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "Polygon":
                    return JsonConvert.DeserializeObject<Polygon>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "MultiPolygon":
                    return JsonConvert.DeserializeObject<MultiPolygon>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "GeometryCollection":
                    return JsonConvert.DeserializeObject<GeometryCollection>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "Feature":
                    return JsonConvert.DeserializeObject<Feature>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                case "FeatureCollection":
                    return JsonConvert.DeserializeObject<FeatureCollection>(sanitizedJson, GeoJSONSettings.SerializerSettings);
                default:
                    Debug.LogError($"Unsupported GeoJSON type: {typeStr}");
                    return null;
            }
        }

        public static string SanitizeInvalidJson(string json)
        {
            string pattern = @"\b(NaN|Infinity|-Infinity|None)\b";
            return Regex.Replace(json, pattern, "'null'");
        }
    }
}
