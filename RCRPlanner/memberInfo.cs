﻿using System;
using System.Collections.Generic;

namespace RCRPlanner
{
    public class memberInfo
    {
        public class FavoutireSeries
        {
            public int series_id { get; set; }
        }
        public class FavoutireCars
        {
            public int car_id { get; set; }
        }
        public class FavoutireTracks
        {
            public int track_id { get; set; }
        }
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Account
        {
            public double ir_dollars { get; set; }
            public double ir_credits { get; set; }
            public string status { get; set; }
        }

        public class CarPackage
        {
            public int package_id { get; set; }
            public List<int> content_ids { get; set; }
        }

        public class DirtOval
        {
            public int category_id { get; set; }
            public string category { get; set; }
            public int license_level { get; set; }
            public double safety_rating { get; set; }
            public double cpi { get; set; }
            public int irating { get; set; }
            public int tt_rating { get; set; }
            public int mpr_num_races { get; set; }
            public string color { get; set; }
            public string group_name { get; set; }
            public int group_id { get; set; }
            public bool pro_promotable { get; set; }
            public int mpr_num_tts { get; set; }
        }

        public class DirtRoad
        {
            public int category_id { get; set; }
            public string category { get; set; }
            public int license_level { get; set; }
            public double safety_rating { get; set; }
            public double cpi { get; set; }
            public int irating { get; set; }
            public int tt_rating { get; set; }
            public int mpr_num_races { get; set; }
            public string color { get; set; }
            public string group_name { get; set; }
            public int group_id { get; set; }
            public bool pro_promotable { get; set; }
            public int mpr_num_tts { get; set; }
        }

        public class Helmet
        {
            public int pattern { get; set; }
            public string color1 { get; set; }
            public string color2 { get; set; }
            public string color3 { get; set; }
            public int face_type { get; set; }
            public int helmet_type { get; set; }
        }

        public class Licenses
        {
            public Oval oval { get; set; }
            public Road road { get; set; }
            public DirtOval dirt_oval { get; set; }
            public DirtRoad dirt_road { get; set; }
        }

        public class Oval
        {
            public int category_id { get; set; }
            public string category { get; set; }
            public int license_level { get; set; }
            public double safety_rating { get; set; }
            public double cpi { get; set; }
            public int irating { get; set; }
            public int tt_rating { get; set; }
            public int mpr_num_races { get; set; }
            public string color { get; set; }
            public string group_name { get; set; }
            public int group_id { get; set; }
            public bool pro_promotable { get; set; }
            public int mpr_num_tts { get; set; }
        }

        public class Restrictions
        {
        }

        public class Road
        {
            public int category_id { get; set; }
            public string category { get; set; }
            public int license_level { get; set; }
            public double safety_rating { get; set; }
            public double cpi { get; set; }
            public int irating { get; set; }
            public int tt_rating { get; set; }
            public int mpr_num_races { get; set; }
            public string color { get; set; }
            public string group_name { get; set; }
            public int group_id { get; set; }
            public bool pro_promotable { get; set; }
            public int mpr_num_tts { get; set; }
        }

        public class Root
        {
            public int cust_id { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public string display_name { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string on_car_name { get; set; }
            public string member_since { get; set; }
            public int last_test_track { get; set; }
            public int last_test_car { get; set; }
            public int last_season { get; set; }
            public int flags { get; set; }
            public int club_id { get; set; }
            public string club_name { get; set; }
            public string connection_type { get; set; }
            public string download_server { get; set; }
            public string last_login { get; set; }
            public DateTime read_comp_rules { get; set; }
            public Account account { get; set; }
            public Helmet helmet { get; set; }
            public Suit suit { get; set; }
            public Licenses licenses { get; set; }
            public List<CarPackage> car_packages { get; set; }
            public List<TrackPackage> track_packages { get; set; }
            public List<int> other_owned_packages { get; set; }
            public bool dev { get; set; }
            public bool alpha_tester { get; set; }
            public bool broadcaster { get; set; }
            public Restrictions restrictions { get; set; }
            public bool has_read_comp_rules { get; set; }
            public string flags_hex { get; set; }
            public bool hundred_pct_club { get; set; }
            public bool twenty_pct_discount { get; set; }
            public bool race_official { get; set; }
            public bool ai { get; set; }
            public DateTime read_tc { get; set; }
            public DateTime read_pp { get; set; }
            public bool has_read_tc { get; set; }
            public bool has_read_pp { get; set; }
            public List<FavoutireSeries> favoutireSeries { get; set; }
            public List<FavoutireCars> favoutireCars { get; set; }
            public List<FavoutireTracks> favoutireTracks { get; set; }
        }

        public class Suit
        {
            public int pattern { get; set; }
            public string color1 { get; set; }
            public string color2 { get; set; }
            public string color3 { get; set; }
            public int body_type { get; set; }
        }

        public class TrackPackage
        {
            public int package_id { get; set; }
            public List<int> content_ids { get; set; }
        }


    }
}
