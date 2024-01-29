using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Types
{
    public class EdgeInformation
    {
        public string LaneId;
        public double Length;
        public double Occupation;
        public byte OffsetToBestLane;
        /// <summary>
        /// 0: lane may not be used for continuing drive, 1: it may be used
        /// </summary>
        public byte laneInformation;
        public List<string> BestSubsequentLanes;
    }
}
