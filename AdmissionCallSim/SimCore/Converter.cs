using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public class Converter
	{
		public static Double ToDB(Double linearPower)
		{
			return 10 * Math.Log10(linearPower);
		}

		public static Double ToLinear(Double dbPower)
		{
			return Math.Pow(10, (dbPower / 10)); 
		}
	}
}
