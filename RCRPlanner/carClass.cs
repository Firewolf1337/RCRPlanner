using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    public class carClass
    {
        public class CarsInClass
        {
            public string car_dirpath { get; set; }
            public int car_id { get; set; }
            public bool retired { get; set; }
        }

        public class Root
        {
            public int car_class_id { get; set; }
            public List<CarsInClass> cars_in_class { get; set; }
            public int cust_id { get; set; }
            public string name { get; set; }
            public int relative_speed { get; set; }
            public string short_name { get; set; }
        }

        public class CarInClassId
        {
            public List<int> car_class_id { get; set; }
            public int car_id { get; set; }
        }
            
    }
}
