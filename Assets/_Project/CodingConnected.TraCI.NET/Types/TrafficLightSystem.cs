using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Types
{
    public class TrafficLightSystem :ComposedTypeBase
    {
        public string TrafficLightSystemId;
        public int TrafficLightSystemLinkIndex;
        public double DistanceToTrafficLightSystem;
        public byte LinkState;
    }
}
