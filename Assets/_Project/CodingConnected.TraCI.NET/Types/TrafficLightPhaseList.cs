using System.Collections.Generic;

namespace CodingConnected.TraCI.NET.Types
{
	public enum PhaseState:byte
	{
		Red = 0x01,
		Yellow = 0x02,
		Green = 0x03,
		OffBlinking = 0x04,
		OffNotBlinking = 0x05
	}

	public class TrafficLightPhase
    { 
		public string PrecRoad { get; set; }
		public string SuccRoad { get; set; }
		public PhaseState Phase { get; set; }
	}

	public class TrafficLightPhaseList : ComposedTypeBase
	{
		public List<TrafficLightPhase> Phases { get; set; }

		public TrafficLightPhaseList()
		{
			Phases = new List<TrafficLightPhase>();
		}
	}
}