using Newtonsoft.Json;

namespace GeoJSON
{
    public static class GeoJSONSettings
    {
        private static JsonSerializerSettings _settings;

        public static JsonSerializerSettings SerializerSettings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new JsonConverter[]
                        {
                            new GeometryConverter()
                        }
                    };
                }
                return _settings;
            }
        }
    }
}
