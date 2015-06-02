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
		// UMTS code are stored into BitMap form
		// IIF array[required] == 0, then accept call and set corresponding bits. When call ends, clear them
		// The code of length 1 require 512 bits, the codes of length 2 require 256 bits, etc, until the codes of length 512 require 1 bit.  
		private Byte[] _array;

		private Byte _byteSize = 8;

		private LSBFormatter _formatter;

		public UMTSCode()
		{
			// The CPICH channel requires the code 256-1, so we reserve it at instanciationss
			_array = new Byte[64] {
				192, 0, 0, 0, 0, 0, 0, 0,
 				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
			};
			_formatter = new LSBFormatter();
		}


		public Boolean requireCode(Int32 maxLength, out Int32 position)
		{
			// We need 512/maxLength bits to fit a request of maxLength
			Int32 numberOfBits = 512 / maxLength;

			try
			{
				// If maxlength is not a power of two, the exception will be caught
				position = checklength(numberOfBits);
				if (position < 0)
				{
					return false;
				}
				else
				{
					reserveBits((UInt32)numberOfBits, position);
					return true;
				}
			}
			catch
			{
				position = -1;
				return false;
			}
		}

		public Boolean freeCode(Int32 len, Int32 position)
		{
			UInt32 numberOfBits = (UInt32)(512 / len);
			try
			{
				freeBits(numberOfBits, position);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		// Reserve concerned bits by setting them in array.
		private void reserveBits(UInt32 numberOfBits, Int32 position)
		{
			// Parameter Checking
			Debug.Assert(position >= 0 && position < 512);
			Debug.Assert(position + numberOfBits <= 512);
			Debug.Assert(isPowerOfTwo(position));

			// get first concerned byte in array
			Byte byte_index = (Byte)(position / _byteSize);
			Byte offset = (Byte)(position % _byteSize);

			// While there is more bits to set
			while (numberOfBits-- > 0)
			{
				// We set the concerned bit in the concerned byte, then increment offset
				_array[byte_index] |= (Byte)(0x01 << offset++);
				// We make sure offset stays inbound
				offset %= _byteSize;
				// If offset revert to zero, we crossed over Byte boundary, increment byte index
				if (offset == 0)
				{
					byte_index++;
				}
			}
		}

		private void freeBits(UInt32 numberOfBits, Int32 position)
		{
			// Parameter Checking
			Debug.Assert(isPowerOfTwo(position));
			Debug.Assert(position >= 0 && position < 512);
			Debug.Assert(position + numberOfBits <= 512);

			// get first concerned byte in array
			Byte byte_index = (Byte)(position / _byteSize);
			// get position concerned inside the byte
			Byte offset = (Byte)(position % _byteSize);

			// While there is more bits to clear
			while (numberOfBits-- > 0)
			{
				// We clear the concerned bit in the concerned byte, then increment offset
				_array[byte_index] &= (Byte)(~(0x01 << offset++));
				// We make sure offset stays inbound
				offset %= _byteSize;
				// If offset revert to zero, we crossed over Byte boundary, increment byte index
				if (offset == 0)
				{
					byte_index++;
				}
			}
		}

		private Boolean isPowerOfTwo(Int32 length)
		{
			// 0 included for model convenience
			return ((length & (length - 1)) == 0);
		}

		// Check if a contiguous portion of the BitMap is available. 
		// If so, returns the index of first location.
		public Int32 checklength(Int32 lengthToCheck)
		{
			Debug.Assert(isPowerOfTwo(lengthToCheck));

			Int32 position = -1;

			// Current byte index in the array
			Int16 byte_index = 0;
			// Offset in current byte
			Byte offset = 0;

			// There is (512 / lengthToCheck) slots into the array, each of them of size lengthToCheck
			Int32 currentSlotIndex = 0;

			// I have no idea why it works.
			while (currentSlotIndex < 512)
			{
				Int32 i = lengthToCheck;
				while (i-- > 0)
				{
					if ((_array[byte_index] & (0x01 << offset)) != 0)
					{
						// If somewhere in the current slot a bit is set, this slot is already reserved.

						// We get to the next slot
						currentSlotIndex += lengthToCheck;

						// We set the byte index from the currentSlot
						byte_index = (Int16)(currentSlotIndex / _byteSize);

						// Tricky : if the slot length is < byte size, we have to increment the offset accordingly.
						// Otherwise, we changed byte, so reset offset to 0
						offset = (lengthToCheck <= _byteSize) ? (Byte)((offset + lengthToCheck) % _byteSize) : (Byte)0;

						goto next;
					}
					else
					{
						// Make offset stay in bounds
						offset++;
						offset %= _byteSize;
						if (offset == 0)
						{
							// Byte boundary crossed, increment byte_index
							byte_index++;
						}
					}
				}

				// We get here if there is no bit set in the current slot
				// This one is free, set the position found at this's slot index
				position = currentSlotIndex;
				// And break to return position
				break;
			next:
				continue;
			}

			return position;
		}

		// Custom ToString printing BitMap in natural order, ie. LSB first for all bytes.
		public override String ToString()
		{
			int i = 0;
			StringBuilder sb = new StringBuilder();
			foreach (Byte b in _array)
			{
				sb.Append(String.Format(_formatter, "{0}", b));
				sb.Append(' ');
				if ((++i % 8) == 0)
				{
					sb.Append('\n');
				}
			}
			return sb.ToString();
		}
	}

	public class LSBFormatter : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
			{
				return this;
			}
			return null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (!this.Equals(formatProvider))
			{
				return null;
			}
			int offset = 0x1;

			Byte obj = (Byte)arg;

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 8; i++)
			{
				if ((obj & (offset << i)) == 0)
				{
					sb.Append('0');
				}
				else
				{
					sb.Append('1');
				}
			}
			return sb.ToString();
		}
	}
}
