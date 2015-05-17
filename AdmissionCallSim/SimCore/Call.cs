using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public class Call
	{
		public enum Type
		{
			NONE,
			VOICE,
			DATA_BAND_L,
			DATA_BAND_H,
		}

		// Call types stored as 
		// Type (Enum) : SIR_Target (Double) : Bandwidth (Double) : Code_Max_Length (Double)
		// To get the type			: Call.getCallTInfos().getKey()
		// To get the SIR			: Call.getCallInfos()[CallType.Type].Item1
		// To get the Bandwidth		: Call.getCallInfos()[CallType.Type].Item2
		// To get the Max CodeLen	: Call.getCallInfos()[CallType.Type].Item3
		private static Dictionary<Type, Tuple<Double, Int32, Int32>> _type_infos;

		private Call()
		{
			// This class is not instanciable
		}

		public static Dictionary<Type, Tuple<Double, Int32, Int32>> getCallInfos()
		{
			if(_type_infos == null)
			{
				// Dictionnary creation
				_type_infos = new Dictionary<Type,Tuple<Double,Int32, Int32>>();
				
				Tuple<Double, Int32, Int32> none_tuple = Tuple.Create<Double, Int32, Int32>(0, 0, 0); // Just in case
				Tuple<Double, Int32, Int32> voice_tuple = Tuple.Create<Double, Int32, Int32>(-20.08, 12000, 256);
				Tuple<Double, Int32, Int32> data_band_l_tuple = Tuple.Create<Double, Int32, Int32>(-13.05, 64000, 32);
				Tuple<Double, Int32, Int32> data_band_h_tuple = Tuple.Create<Double, Int32, Int32>(-10.54, 144000, 16);
				
				_type_infos.Add(Type.NONE, none_tuple);
				_type_infos.Add(Type.VOICE, voice_tuple);
				_type_infos.Add(Type.DATA_BAND_L, data_band_l_tuple);
				_type_infos.Add(Type.DATA_BAND_H, data_band_h_tuple);
			}

			return _type_infos;
		}

	}
}
