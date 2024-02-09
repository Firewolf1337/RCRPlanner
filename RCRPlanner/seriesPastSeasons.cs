using System.Collections.Generic;


namespace RCRPlanner
{
    public class seriesPastSeasons
    {
        public class AllowedLicense
        {
            public int parent_id { get; set; }
            public int license_group { get; set; }
            public int min_license_level { get; set; }
            public int max_license_level { get; set; }
            public string group_name { get; set; }
        }

        public class CarClass
        {
            public int car_class_id { get; set; }
            public string short_name { get; set; }
            public string name { get; set; }
            public int relative_speed { get; set; }
        }

        public class LicenseGroupType
        {
            public int license_group_type { get; set; }
        }

        public class RaceWeek
        {
            public int season_id { get; set; }
            public int race_week_num { get; set; }
            public Track track { get; set; }
        }

        public class Root
        {
            public bool success { get; set; }
            public Series series { get; set; }
            public int series_id { get; set; }
        }

        public class Season
        {
            public int season_id { get; set; }
            public int series_id { get; set; }
            public string season_name { get; set; }
            public string season_short_name { get; set; }
            public int season_year { get; set; }
            public int season_quarter { get; set; }
            public bool active { get; set; }
            public bool official { get; set; }
            public bool driver_changes { get; set; }
            public bool fixed_setup { get; set; }
            public int license_group { get; set; }
            public List<LicenseGroupType> license_group_types { get; set; }
            public List<CarClass> car_classes { get; set; }
            public List<RaceWeek> race_weeks { get; set; }
        }

        public class Series
        {
            public int series_id { get; set; }
            public string series_name { get; set; }
            public string series_short_name { get; set; }
            public int category_id { get; set; }
            public string category { get; set; }
            public bool active { get; set; }
            public bool official { get; set; }
            public bool fixed_setup { get; set; }
            public string search_filters { get; set; }
            public string logo { get; set; }
            public int license_group { get; set; }
            public List<LicenseGroupType> license_group_types { get; set; }
            public List<AllowedLicense> allowed_licenses { get; set; }
            public List<Season> seasons { get; set; }
        }

        public class Track
        {
            public int track_id { get; set; }
            public string track_name { get; set; }
            public string config_name { get; set; }
        }

    }
}
