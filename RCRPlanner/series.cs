using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    public class series
    {
        public class AllowedLicense
        {
            public int license_group { get; set; }
            public int min_license_level { get; set; }
            public int max_license_level { get; set; }
            public string group_name { get; set; }
        }

        public class Root
        {
            public List<AllowedLicense> allowed_licenses { get; set; }
            public string category { get; set; }
            public int category_id { get; set; }
            public bool eligible { get; set; }
            public int max_starters { get; set; }
            public int min_starters { get; set; }
            public int oval_caution_type { get; set; }
            public int road_caution_type { get; set; }
            public string search_filters { get; set; }
            public int series_id { get; set; }
            public string series_name { get; set; }
            public string series_short_name { get; set; }
            public string forum_url { get; set; }
        }

    }
}
