using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public class UMTSCode
	{
		// UMTS code stored into BitMap form
		// IIF array[required] == 0, then accept call and set corresponding bits
		// When call ends, clear bits.
		private Byte[] array = new Byte[64] {
			0, 0, 0, 0, 0, 0, 0, 0,
 			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0,
		};

		// We should also find a storing mechanism to know which codes each user require

		public Boolean requireCode(Int32 maxLength, out Int32 position)
		{
			position = 1;
			return true;
		}
	}
}
