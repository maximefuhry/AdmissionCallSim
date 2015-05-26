using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AdmissionCallSim.SimCore
{
	public class Antenna
	{
		public Double MaxPower { get; set; }

		public Double CurrentPower { get; set; }

		public Double Gain { get; set; }

		public Double Loss { get; set; }

		public Double SignalingChannelPower { get; set; }

		// Power in Watt
		private readonly Double _defaultMaxPower = 0.200;

		public Double X { get; set; }

		public Double Y { get; set; }

		//private Double _call_range;

		public Antenna() : this(0, 0) 
		{

		}

		public Antenna(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
			Gain = 2.5;
			Loss = 0;
			MaxPower = _defaultMaxPower;
			SignalingChannelPower = MaxPower / 10;
			CurrentPower += SignalingChannelPower;
		}
	}
}
