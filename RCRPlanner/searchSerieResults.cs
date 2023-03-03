using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    class searchSerieResults
    {
        public class Root
        {
            public int session_id { get; set; }
            public int subsession_id { get; set; }
            public DateTime start_time { get; set; }
            public DateTime end_time { get; set; }
            public int license_category_id { get; set; }
            public string license_category { get; set; }
            public int num_drivers { get; set; }
            public int num_cautions { get; set; }
            public int num_caution_laps { get; set; }
            public int num_lead_changes { get; set; }
            public int event_laps_complete { get; set; }
            public bool driver_changes { get; set; }
            public int winner_group_id { get; set; }
            public string winner_name { get; set; }
            public bool winner_ai { get; set; }
            public Track track { get; set; }
            public bool official_session { get; set; }
            public int season_id { get; set; }
            public int season_year { get; set; }
            public int season_quarter { get; set; }
            public int event_type { get; set; }
            public string event_type_name { get; set; }
            public int series_id { get; set; }
            public string series_name { get; set; }
            public string series_short_name { get; set; }
            public int race_week_num { get; set; }
            public int event_strength_of_field { get; set; }
            public int event_average_lap { get; set; }
            public int event_best_lap_time { get; set; }
        }

        public class Track
        {
            public int track_id { get; set; }
            public string track_name { get; set; }
            public string config_name { get; set; }
        }
    }
}
