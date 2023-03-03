using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    public class seriesSeason
    {
        public class CarRestriction
        {
            public int car_id { get; set; }
            public int max_pct_fuel_fill { get; set; }
            public int weight_penalty_kg { get; set; }
            public double power_adjust_pct { get; set; }
            public int max_dry_tire_sets { get; set; }
            public int? race_setup_id { get; set; }
            public int? qual_setup_id { get; set; }
        }

        public class CarType
        {
            public string car_type { get; set; }
        }

        public class HeatSesInfo
        {
            public int heat_info_id { get; set; }
            public int cust_id { get; set; }
            public bool hidden { get; set; }
            public DateTime created { get; set; }
            public string heat_info_name { get; set; }
            public string description { get; set; }
            public int max_entrants { get; set; }
            public int race_style { get; set; }
            public bool open_practice { get; set; }
            public int pre_qual_practice_length_minutes { get; set; }
            public int pre_qual_num_to_main { get; set; }
            public int qual_style { get; set; }
            public int qual_length_minutes { get; set; }
            public int qual_laps { get; set; }
            public int qual_num_to_main { get; set; }
            public int qual_scoring { get; set; }
            public int qual_caution_type { get; set; }
            public int qual_open_delay_seconds { get; set; }
            public bool qual_scores_champ_points { get; set; }
            public int heat_length_minutes { get; set; }
            public int heat_laps { get; set; }
            public int heat_max_field_size { get; set; }
            public int heat_num_position_to_invert { get; set; }
            public int heat_caution_type { get; set; }
            public int heat_num_from_each_to_main { get; set; }
            public bool heat_scores_champ_points { get; set; }
            public int consolation_num_to_consolation { get; set; }
            public int consolation_num_to_main { get; set; }
            public int consolation_first_max_field_size { get; set; }
            public int consolation_first_session_length_minutes { get; set; }
            public int consolation_first_session_laps { get; set; }
            public int consolation_delta_max_field_size { get; set; }
            public int consolation_delta_session_length_minutes { get; set; }
            public int consolation_delta_session_laps { get; set; }
            public int consolation_num_position_to_invert { get; set; }
            public bool consolation_scores_champ_points { get; set; }
            public bool consolation_run_always { get; set; }
            public int pre_main_practice_length_minutes { get; set; }
            public int main_length_minutes { get; set; }
            public int main_laps { get; set; }
            public int main_max_field_size { get; set; }
            public int main_num_position_to_invert { get; set; }
            public int heat_session_minutes_estimate { get; set; }
        }

        public class LicenseGroupType
        {
            public int license_group_type { get; set; }
        }

        public class RaceTimeDescriptor
        {
            public bool repeating { get; set; }
            public bool super_session { get; set; }
            public int session_minutes { get; set; }
            public string start_date { get; set; }
            public List<int> day_offset { get; set; }
            public string first_session_time { get; set; }
            public int repeat_minutes { get; set; }
            public List<DateTime> session_times { get; set; }
        }

        public class Root
        {
            public bool active { get; set; }
            public List<int> car_class_ids { get; set; }
            public List<CarType> car_types { get; set; }
            public bool caution_laps_do_not_count { get; set; }
            public bool complete { get; set; }
            public bool cross_license { get; set; }
            public int driver_change_rule { get; set; }
            public bool driver_changes { get; set; }
            public int drops { get; set; }
            public bool fixed_setup { get; set; }
            public int green_white_checkered_limit { get; set; }
            public bool grid_by_class { get; set; }
            public bool ignore_license_for_practice { get; set; }
            public int incident_limit { get; set; }
            public int incident_warn_mode { get; set; }
            public int incident_warn_param1 { get; set; }
            public int incident_warn_param2 { get; set; }
            public bool is_heat_racing { get; set; }
            public int license_group { get; set; }
            public List<LicenseGroupType> license_group_types { get; set; }
            public bool lucky_dog { get; set; }
            public int max_team_drivers { get; set; }
            public int max_weeks { get; set; }
            public int min_team_drivers { get; set; }
            public bool multiclass { get; set; }
            public bool must_use_diff_tire_types_in_race { get; set; }
            public object next_race_session { get; set; }
            public int num_opt_laps { get; set; }
            public bool official { get; set; }
            public int op_duration { get; set; }
            public int open_practice_session_type_id { get; set; }
            public bool qualifier_must_start_race { get; set; }
            public int race_week { get; set; }
            public int race_week_to_make_divisions { get; set; }
            public int reg_user_count { get; set; }
            public bool region_competition { get; set; }
            public bool restrict_by_member { get; set; }
            public bool restrict_to_car { get; set; }
            public bool restrict_viewing { get; set; }
            public string schedule_description { get; set; }
            public List<Schedule> schedules { get; set; }
            public int season_id { get; set; }
            public string season_name { get; set; }
            public int season_quarter { get; set; }
            public string season_short_name { get; set; }
            public int season_year { get; set; }
            public bool send_to_open_practice { get; set; }
            public int series_id { get; set; }
            public DateTime start_date { get; set; }
            public bool start_on_qual_tire { get; set; }
            public bool start_zone { get; set; }
            public List<TrackType> track_types { get; set; }
            public int unsport_conduct_rule_mode { get; set; }
            public string rookie_season { get; set; }
            public HeatSesInfo heat_ses_info { get; set; }
            public int? reg_open_minutes { get; set; }
            public int? race_points { get; set; }
        }

        public class Schedule
        {
            public int season_id { get; set; }
            public int race_week_num { get; set; }
            public int series_id { get; set; }
            public string series_name { get; set; }
            public string season_name { get; set; }
            public string schedule_name { get; set; }
            public string start_date { get; set; }
            public int simulated_time_multiplier { get; set; }
            public int? race_lap_limit { get; set; }
            public int? race_time_limit { get; set; }
            public string start_type { get; set; }
            public string restart_type { get; set; }
            public bool qual_attached { get; set; }
            public bool full_course_cautions { get; set; }
            public object special_event_type { get; set; }
            public bool start_zone { get; set; }
            public Track track { get; set; }
            public Weather weather { get; set; }
            public TrackState track_state { get; set; }
            public List<RaceTimeDescriptor> race_time_descriptors { get; set; }
            public List<CarRestriction> car_restrictions { get; set; }
        }

        public class Track
        {
            public int track_id { get; set; }
            public string track_name { get; set; }
            public string config_name { get; set; }
            public int category_id { get; set; }
            public string category { get; set; }
        }

        public class TrackState
        {
            public bool leave_marbles { get; set; }
            public int? practice_rubber { get; set; }
            public int? warmup_rubber { get; set; }
            public int? race_rubber { get; set; }
        }

        public class TrackType
        {
            public string track_type { get; set; }
        }

        public class Weather
        {
            public int version { get; set; }
            public int type { get; set; }
            public int temp_units { get; set; }
            public int temp_value { get; set; }
            public int rel_humidity { get; set; }
            public int fog { get; set; }
            public int wind_dir { get; set; }
            public int wind_units { get; set; }
            public int wind_value { get; set; }
            public int skies { get; set; }
            public int weather_var_initial { get; set; }
            public int weather_var_ongoing { get; set; }
            public int time_of_day { get; set; }
            public DateTime simulated_start_time { get; set; }
            public List<int> simulated_time_offsets { get; set; }
            public int simulated_time_multiplier { get; set; }
            public DateTime simulated_start_utc_time { get; set; }
        }
    }
}
