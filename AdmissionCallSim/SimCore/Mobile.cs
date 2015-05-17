using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public partial class Mobile
	{
		private static Int32 _nbrmobiles = 0;

		private Int32 _id;
		public Int32 ID
		{
			get { return _id; }
			set { _id = value; }
		}

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
			get { return _x; }
			set { _x = value; }
		}

		private Int32 _y;
		public Int32 Y
		{
			get { return _y; }
			set { _y = value; }
		}

		private Int32 _calllength = 0;
		public Int32 CallLength
		{
			get { return _calllength; }
			set { _calllength = value; }
		}

		private Cell _nearestCell = null;
		public Cell NearestCell
		{
			get { return _nearestCell; }
			set { _nearestCell = value; }
		}

		private Call.Type _type;
		public Call.Type Type
		{
			get { return _type; }
		//	set { _type = value; }
		}

		public static Int32 getNbrMobiles()
		{
			return _nbrmobiles;
		}

		public Int32 startCall(Call.Type t, Int32 length)
		{
			Int32 result = _nearestCell.requestCall(this, t);
			if (result != (Int32) CallResult.FAILURE)
			{
				// call request succeded
				_calllength = length;
				_type = t;
			}
			return result;
		}

		public void runCall()
		{
			_calllength--;
		}

		public void endCall()
		{
			_nearestCell.endCall(this, _type);
			// Call length is already 0 if we step into here
			_type = Call.Type.NONE;
		}
	}
}
