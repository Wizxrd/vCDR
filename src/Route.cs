using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vCDR.src
{
    public class Route
    {
        public string Orig { get; set; }
        public string Dest { get; set; }
        public string Nav { get; set; }
        public string Coordination { get; set; }
        public string Crossing { get; set; }
        public string DepartureFix { get; set; }
        public string CodedRoute { get; set; }
        public string Altitude { get; set; }
        public string Note { get; set; }
    }
}
