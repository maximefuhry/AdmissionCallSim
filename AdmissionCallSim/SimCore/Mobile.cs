using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	class Mobile
	{
		private static Int32 _id = 0;

		// Emitted power
		private Double _Pe;
		public Double PE
		{
			get { return _Pe; }
		}

		private Double _default_Pe = 40;

		private Int32 _x;
		public Int32 X
		{
			get { return X; }
			set { _x = value; }
		}

		private Int32 _y;
		public Int32 Y
		{
			get { return _y; }
			set { _y = value; }
		}

		private Int32 _callduration = 0;

		private Cell _nearestCell = null;
		public Cell NearestCell
		{
			get { return _nearestCell; }
			set { _nearestCell = value; }
		}

		private CallType.Type _type;

		public Mobile() : this(0,0)
		{

		}

		public Mobile(Int32 x, Int32 y)
		{
			_x = x;
			_y = y;
			_Pe = _default_Pe;
			_type = CallType.Type.NONE;
			_id++;
		}

		public Int32 getDuration()
		{
			return _callduration;
		}

		public Int32 startCall(CallType.Type type, Int32 duration)
		{
			Int32 result = _nearestCell.requestCall(this, type);
			if (result != (Int32) CallResult.FAILURE)
			{
				// call request succeded
				_callduration = duration;
				_type = type;
			}
			return result;
		}

		public void runCall()
		{
			_callduration--;
		}

		public void endCall()
		{
			_nearestCell.endCall(this, _type);
		}
	}
}
