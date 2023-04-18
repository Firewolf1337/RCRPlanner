using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    public class participationCredits
    {
        public class Root
        {
            public int cust_id { get; set; }
            public int season_id { get; set; }
            public int series_id { get; set; }
            public string series_name { get; set; }
            public int license_group { get; set; }
            public string license_group_name { get; set; }
            public int participation_credits { get; set; }
            public int min_weeks { get; set; }
            public int weeks { get; set; }
            public int earned_credits { get; set; }
            public int total_credits { get; set; }
        }
    }
}
