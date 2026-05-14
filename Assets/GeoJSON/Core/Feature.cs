using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeoJSON
{
    public class Feature : GeoJSONObject
    {
        [JsonProperty("geometry")]
        public Geometry geometry { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, JToken> properties { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        [JsonConstructor]
        public Feature()
        {
            type = GeoJSONObjectType.Feature;
            properties = new Dictionary<string, JToken>();
        }

        public T GetProperty<T>(string key)
        {
            if (properties.ContainsKey(key))
            {
                return properties[key].ToObject<T>();
            }
            return default(T);
        }
    }

    public class FeatureCollection : GeoJSONObject
    {
        [JsonProperty("features")]
        public List<Feature> features { get; set; }

        [JsonConstructor]
        public FeatureCollection()
        {
            type = GeoJSONObjectType.FeatureCollection;
            features = new List<Feature>();
        }
    }
}
