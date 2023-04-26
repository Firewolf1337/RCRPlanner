using System;
using System.Collections.Generic;

namespace RCRPlanner
{
    public class cars
    {

        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);

        public class CarsInSeries
        {
            public int car_id { get; set; }
            public int series_id { get; set; }
        }
        public class CarType
        {
            public string car_type { get; set; }
        }

        public class paintRules
        {
            public bool PaintCarAvailable { get; set; }
            public string Color1 { get; set; }
            public string Color2 { get; set; }
            public string Color3 { get; set; }
            public bool Sponsor1Available { get; set; }
            public bool Sponsor2Available { get; set; }
            public string Sponsor1 { get; set; }
            public string Sponsor2 { get; set; }
            public bool PaintWheelAvailable { get; set; }
            public string WheelColor { get; set; }
            public bool RimTypeAvailable { get; set; }
            public string RimType { get; set; }
            public bool AllowNumberFontChanges { get; set; }
            public string NumberFont { get; set; }
            public bool AllowNumberColorChanges { get; set; }
            public string NumberColor1 { get; set; }
            public string NumberColor2 { get; set; }
            public string NumberColor3 { get; set; }
            public string RulesExplanation { get; set; }
        }
        public class PaintRules
        {
            public Dictionary<string, paintRules> paint_RulesClass { get; set; }
            public bool? RestrictCustomPaint { get; set; }
        }

        public class Root
        {
            public bool ai_enabled { get; set; }
            public bool allow_number_colors { get; set; }
            public bool allow_number_font { get; set; }
            public bool allow_sponsor1 { get; set; }
            public bool allow_sponsor2 { get; set; }
            public bool allow_wheel_color { get; set; }
            public bool award_exempt { get; set; }
            public string car_dirpath { get; set; }
            public int car_id { get; set; }
            public string car_name { get; set; }
            public string car_name_abbreviated { get; set; }
            public List<CarType> car_types { get; set; }
            public int car_weight { get; set; }
            public List<string> categories { get; set; }
            public DateTime created { get; set; }
            public bool free_with_subscription { get; set; }
            public string forum_url { get; set; }
            public bool has_headlights { get; set; }
            public bool has_multiple_dry_tire_types { get; set; }
            public int hp { get; set; }
            public int max_power_adjust_pct { get; set; }
            public int max_weight_penalty_kg { get; set; }
            public int min_power_adjust_pct { get; set; }
            public int package_id { get; set; }
            public int patterns { get; set; }
            public double price { get; set; }
            public bool retired { get; set; }
            public string search_filters { get; set; }
            public int sku { get; set; }
            public string car_make { get; set; }
            public string car_model { get; set; }
            public string site_url { get; set; }
            //public PaintRules paint_rules { get; set; }

        }


    }
}
