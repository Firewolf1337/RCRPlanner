using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    public class searchSeries
    {
        public class ChunkInfo
        {
            public int chunk_size { get; set; }
            public int num_chunks { get; set; }
            public int rows { get; set; }
            public string base_download_url { get; set; }
            public List<string> chunk_file_names { get; set; }
        }

        public class Data
        {
            public bool success { get; set; }
            public ChunkInfo chunk_info { get; set; }
            public Params @params { get; set; }
        }

        public class Params
        {
            public List<int> category_ids { get; set; }
            public int series_id { get; set; }
            public int season_year { get; set; }
            public int season_quarter { get; set; }
            public int race_week_num { get; set; }
            public bool official_only { get; set; }
            public List<int> event_types { get; set; }
        }

        public class Root
        {
            public string type { get; set; }
            public Data data { get; set; }
        }
    }
}
