﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public partial class Mobile
	{
		public static Int32 Nbrmobiles { get; set;}

		public Int32 ID { get; set; }

		// Emitted power
		public MobileClass Class { get; set; }

		public Double Gain { get; set; }

		public Double Loss { get; set; }

		public Int32 X { get; set; }

		public Int32 Y { get; set; }

		// the distance with the BS
		public Double Distance { get; set; }

		public Int32 CallLength { get; set; }

		public Cell NearestCell { get; set; }

		public Call.Type Type { get; set; }

		public CallResult startCall(Call.Type t, Int32 length)
		{
			CallResult result = NearestCell.requestCall(this, t);
			//CallResult result = CallResult.FAILURE;
			if (result != CallResult.FAILURE)
			{
				// call request succeded
				CallLength = length;
				Type = t;
			}
			return result;
		}

		public void runCall()
		{
			CallLength--;
		}

		public void endCall()
		{
			NearestCell.endCall(this, Type);
			// Call length is already 0 if we step into here
			Type = Call.Type.NONE;
		}

		public static Double getPower(MobileClass mobileClass)
		{
			switch (mobileClass)
			{
				case MobileClass.LOW:
					return 125 * Math.Pow(10, -3);
				case MobileClass.HIGH:
					return 250 * Math.Pow(10, -3);
				case MobileClass.MEDIUM:
					return 500 * Math.Pow(10, -3);
				default:
					return Double.MinValue;
			}
		}
	}

	public enum MobileClass
	{
		LOW,
		MEDIUM,
		HIGH
	}
}
