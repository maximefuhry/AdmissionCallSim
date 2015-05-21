using System;
using System.Diagnostics;
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
		private Byte[] _array = new Byte[64] {
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

		public Boolean requireCode(UInt32 maxLength, out UInt32 position)
		{
			position = 1;
			return true;
		}

		private void reserveBits(UInt32 numberOfBits, UInt32 position)
		{
			// Boundary Checking
			Debug.Assert(position >= 0 && position < 512);
			Debug.Assert(position + numberOfBits < 512);
			Debug.Assert(isPowerOfTwo(position));
			
			// get first concerned byte in array
			Byte byte_index = (Byte) (position / sizeof(Byte));
			Byte offset = (Byte) (position % sizeof(Byte));

			while (numberOfBits-- > 0)
			{
				// We set the concerned bit in the concerned byte, then increment offset
				_array[byte_index] |= (Byte) (0x01 << offset++);
				// We make sure offset stays inbound
				offset %= sizeof(Byte);
				// If offset revert to zero, it means we crossed over Byte boundary, increment byte index
				if (offset == 0)
				{
					byte_index++;
				}
			}
		}

		private Boolean isPowerOfTwo(UInt32 length)
		{
			return (length != 0) && ((length & (length - 1)) == 0);
		}
		
		private Int32 checklength(UInt32 lengthToCheck)
		{
			Debug.Assert(isPowerOfTwo(lengthToCheck));
			// The UMTS codes are available under BitMap form
			// It implies the code of length 1 require 512 bits, the codes length 2 require 256 bits, etc, until the code length 512 require 1 bit.  

			// IIF sizeof(_array) == 512, then there is sizeof(_array) / length possibilities into the array.
			Int32 position = -1;
			Byte byte_index = 0;
			Byte offset = 0;

			UInt32 j = 0;

			// Huuhh, should work (in theory).
			while (j < 512)
			{
				UInt32 i = lengthToCheck;
				while (i-- > 0)
				{
					if ((_array[byte_index] & (0x01 << offset++)) != 0 )
					{
						goto notfound;
					}
					else
					{
						offset %= sizeof(Byte);
						if (offset == 0)
						{
							// Byte boundary crossed, increment byte_index
							byte_index++;
						}
					}
				}
found:
				position = j;
				break;
notfound:
				j += lengthToCheck;
				byte_index = j;
				offset = 0; ;
			}

			return position;
		}

		public String ToString()
		{
			return _array.ToString();
		}
	}
}
