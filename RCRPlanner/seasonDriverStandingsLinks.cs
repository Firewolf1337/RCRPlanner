using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    class seasonDriverStandingsLinks
    {
        public class ChunkInfo
        {
            public int chunk_size { get; set; }
            public int num_chunks { get; set; }
            public int rows { get; set; }
            public string base_download_url { get; set; }
            public List<string> chunk_file_names { get; set; }
        }

        public class Root
        {
            public bool success { get; set; }
            public int season_id { get; set; }
            public string season_name { get; set; }
            public string season_short_name { get; set; }
            public int series_id { get; set; }
            public string series_name { get; set; }
            public int car_class_id { get; set; }
            public int race_week_num { get; set; }
            public int division { get; set; }
            public int club_id { get; set; }
            public int customer_rank { get; set; }
            public ChunkInfo chunk_info { get; set; }
            public string last_updated { get; set; }
        }
    }
}
