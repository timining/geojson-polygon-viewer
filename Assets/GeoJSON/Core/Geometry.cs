using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeoJSON
{
    public abstract class Geometry : GeoJSONObject
    {
    }

    public class Point : Geometry
    {
        [JsonProperty("coordinates")]
        public List<double> coordinates { get; set; }

        [JsonConstructor]
        public Point()
        {
            type = GeoJSONObjectType.Point;
        }
    }

    public class LineString : Geometry
    {
        [JsonProperty("coordinates")]
        public List<List<double>> coordinates { get; set; }

        [JsonConstructor]
        public LineString()
        {
            type = GeoJSONObjectType.LineString;
        }
    }

    public class Polygon : Geometry
    {
        [JsonProperty("coordinates")]
        public List<List<List<double>>> coordinates { get; set; }

        [JsonConstructor]
        public Polygon()
        {
            type = GeoJSONObjectType.Polygon;
        }
    }

    public class MultiPoint : Geometry
    {
        [JsonProperty("coordinates")]
        public List<List<double>> coordinates { get; set; }

        [JsonConstructor]
        public MultiPoint()
        {
            type = GeoJSONObjectType.MultiPoint;
        }
    }

    public class MultiLineString : Geometry
    {
        [JsonProperty("coordinates")]
        public List<List<List<double>>> coordinates { get; set; }

        [JsonConstructor]
        public MultiLineString()
        {
            type = GeoJSONObjectType.MultiLineString;
        }
    }

    public class MultiPolygon : Geometry
    {
        [JsonProperty("coordinates")]
        public List<List<List<List<double>>>> coordinates { get; set; }

        [JsonConstructor]
        public MultiPolygon()
        {
            type = GeoJSONObjectType.MultiPolygon;
        }
    }

    public class GeometryCollection : Geometry
    {
        [JsonProperty("geometries")]
        public List<Geometry> geometries { get; set; }

        [JsonConstructor]
        public GeometryCollection()
        {
            type = GeoJSONObjectType.GeometryCollection;
        }
    }
}
