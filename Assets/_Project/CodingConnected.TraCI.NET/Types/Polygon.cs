using System.Collections.Generic;

namespace CodingConnected.TraCI.NET.Types
{
	public class Polygon : ComposedTypeBase
	{
		public List<Position2D> Points { get; set; }

		public Polygon()
		{
			Points = new List<Position2D>();
		}
	}
}