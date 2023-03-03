using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    class seasonDriverStandings
    {
        public class Helmet
        {
            public int pattern { get; set; }
            public string color1 { get; set; }
            public string color2 { get; set; }
            public string color3 { get; set; }
            public int face_type { get; set; }
            public int helmet_type { get; set; }
        }

        public class License
        {
            public int category_id { get; set; }
            public string category { get; set; }
            public int license_level { get; set; }
            public double safety_rating { get; set; }
            public int irating { get; set; }
            public string color { get; set; }
            public string group_name { get; set; }
            public int group_id { get; set; }
        }

        public class Root
        {
            public int rank { get; set; }
            public int cust_id { get; set; }
            public string display_name { get; set; }
            public int division { get; set; }
            public int club_id { get; set; }
            public string club_name { get; set; }
            public License license { get; set; }
            public Helmet helmet { get; set; }
            public int weeks_counted { get; set; }
            public int starts { get; set; }
            public int wins { get; set; }
            public int top5 { get; set; }
            public int top25_percent { get; set; }
            public int poles { get; set; }
            public int avg_start_position { get; set; }
            public int avg_finish_position { get; set; }
            public int avg_field_size { get; set; }
            public int laps { get; set; }
            public int laps_led { get; set; }
            public int incidents { get; set; }
            public int points { get; set; }
            public bool week_dropped { get; set; }
            public string country_code { get; set; }
            public string country { get; set; }
        }
    }
}
