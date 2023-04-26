using System.Text.Json.Serialization;


namespace RCRPlanner
{
    public class trackAssets
    {
        public class Root
        {
            public string coordinates { get; set; }
            public string detail_copy { get; set; }
            public string detail_techspecs_copy { get; set; }
            public string detail_video { get; set; }
            public string folder { get; set; }
            public string gallery_images { get; set; }
            public string gallery_prefix { get; set; }
            public string large_image { get; set; }
            public string logo { get; set; }
            public string north { get; set; }
            public int num_svg_images { get; set; }
            public string small_image { get; set; }
            public int track_id { get; set; }
            public string track_map { get; set; }
            public TrackMapLayers track_map_layers { get; set; }
        }
        public class TrackMapLayers
        {
            public string background { get; set; }
            public string inactive { get; set; }
            public string active { get; set; }
            public string pitroad { get; set; }
            [JsonPropertyName("start-finish")]
            public string StartFinish { get; set; }
            public string turns { get; set; }
        }
    }
}
