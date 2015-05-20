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
		private Double _maxPower;
		public Double MaxPower
		{
			get { return _maxPower; }
		}

		private Double _currentPower;
		public Double CurrentPower
		{
			get { return _currentPower; }
			set { _currentPower = value; }
		}

		// All powers in dBm
		private readonly Double _defaultMaxPower = 53;

		private Double _x;
		public Double X
		{
			get { return _x; }
			set { _x = value; }
		}

		private Double _y;
		public Double Y
		{
			get { return _y; }
			set { _y = value; }
		}

		//private Double _call_range;

		public Antenna() : this(0, 0) 
		{

		}

		public Antenna(Int32 x, Int32 y)
		{
			_x = x;
			_y = y;
			_maxPower = _defaultMaxPower;
			_currentPower = 0;
		}

	}
}
