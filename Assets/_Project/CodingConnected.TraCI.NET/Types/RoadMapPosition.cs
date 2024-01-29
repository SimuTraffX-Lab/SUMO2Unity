namespace CodingConnected.TraCI.NET.Types
{
	public class RoadMapPosition : ComposedTypeBase
	{
		public string RoadId { get; set; }
		public double Pos { get; set; }
		public byte LaneId { get; set; }
	}
}