using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	class CallType
	{
		public enum Type
		{
			NONE,
			VOICE,
			STREAMING,
			DATA,
			MESSAGE
		}

		// Call types stored as 
		// Type (Enum) : SIR_Target (Double) : Bandwith (Double)
		// To get the type		 : CallType.getCallTypeInfos().getKey()
		// To get the SIR		 : CallType.getCallTypeInfos()[CallType.Type].Item1
		// To get the Bandwidth  : CallType.getCallTypeInfos()[CallType.Type].Item2
		private static Dictionary<Type, Tuple<Double, Double>> _type_infos;

		private CallType()
		{
			// This class is not instanciable
		}

		public static Dictionary<Type, Tuple<Double, Double>> getCallTypeInfos()
		{
			if(_type_infos == null)
			{
				_type_infos = new Dictionary<Type,Tuple<Double,Double>>();
				// TODO : fill in the call type informations
			}
			return _type_infos;
		}

	}
}
