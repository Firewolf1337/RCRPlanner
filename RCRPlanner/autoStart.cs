using System.Collections.Generic;

namespace RCRPlanner
{
    public class autoStart
    {

        public class Root
        {
            public bool Active { get; set; }
            public bool Minimized { get; set; }
            public bool Kill { get; set; }
            public bool KillByName { get; set; }
            public List<Programs> Programs {get; set;}

        }
        public class Programs { 
            public int ID { get; set; }
            public string Path { get; set; }
            public bool Paused { get; set; } = false;

        }
    }
}
