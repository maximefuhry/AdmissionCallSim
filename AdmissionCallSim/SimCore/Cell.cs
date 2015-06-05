using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public class Cell
	{
		private static Double _thermalNoise;
	
		private Int32 _frequency;

		private Int32 _defaultFrequency = (Int32)(2100 * Math.Pow(10,6));

		//private Double _cellInterference;

		//private readonly Double _umtsBandwidth = 3840000;

		private static Double _c;

		public Antenna Antenna { get; set; }


		// This list is representing 
		// ALL MOBILES CURRENTLY USING THIS CELL'S ANTENNA
		// with the power required, the code length & position
		public Dictionary<Mobile, Tuple<Double, Int32, Int32>> CallingMobiles { get; private set; }

		// List of pending mobiles with power required
		public Dictionary<Mobile, Double> PendingMobiles { get; set; }

		// To replace _codeArray short to mid-term;
		public UMTSCode UMTSCodes { get; set; }

		public Cell() : 
			this(0, 0)
		{

		}

		// Constructorss
		public Cell(Int32 x, Int32 y)
		{
			Antenna = new Antenna(x,y);
			_thermalNoise = Converter.ToLinear(-129);
			_c = 3 * Math.Pow(10, 8);
			_frequency = _defaultFrequency;
			CallingMobiles = new Dictionary<Mobile,Tuple<Double,Int32,Int32>>();
			//_cellInterference = 0;
			UMTSCodes = new UMTSCode();
		}

		// This method checks if the cell is able to accept the new call
		// if it is, then we wait for a code to be available
 		// and return it to the mobile
		public CallResult requestCall(Mobile m, Call.Type service)
		{
			Debug.Assert(service != Call.Type.NONE);

			Double SIR = computeSIR(frissPower(Antenna.SignalingChannelPower, m), m);

			Double requiredPower = getRequiredPower(SIR, service);

			if (!doAdmissionControl(requiredPower, SIR, service, m.Class))
			{
				return CallResult.FAILURE;
			}

			// The network is OK, check for for code
			Int32 codeLength = Call.getCallInfos()[service].Item3;
			Int32 position;

			//Boolean hasCode = UMTSCodes.requireCode(codeLength, out position);

			if (UMTSCodes.requireCode(codeLength, out position))
			{
				// The code is attributed, the mobile can start using the cell ressources
				CallingMobiles.Add(m, Tuple.Create<Double, Int32, Int32>(requiredPower, codeLength, position));
				
				// Register current mobile for calling
				Antenna.CurrentPower += requiredPower;
				
				return CallResult.SUCCESS;
				
			}
			else
			{
				// There is no code left, add to pending list to save requiredPower
				PendingMobiles.Add(m, requiredPower);

				return CallResult.PENDING;
			}
		}

		private Double getRequiredPower(Double SIR, Call.Type type)
		{
			return Antenna.SignalingChannelPower * (Converter.ToLinear(Call.getCallInfos()[type].Item1) / SIR);
		}

		private Double frissPower(Double power, Mobile m)
		{
			return ((power * Antenna.Gain * m.Gain) / (Antenna.Loss * m.Loss)) * Math.Pow((_c / (2 * Math.PI * _frequency * Math.Pow(m.Distance,2))), 2);
		}

		public void endCall(Mobile m)
		{
			Antenna.CurrentPower -= CallingMobiles[m].Item1;
			UMTSCodes.freeCode(CallingMobiles[m].Item2, CallingMobiles[m].Item3);
			CallingMobiles.Remove(m);
		}

		private Boolean doAdmissionControl(Double requiredPower, Double computedSIR, Call.Type type, MobileClass mobileClass)
		{
			 if (computedSIR < Call.getCallInfos()[type].Item1)
			{
				return false;
			}
			if (requiredPower >= Antenna.SignalingChannelPower)
			{
				return false;
			}
			if (requiredPower + Antenna.CurrentPower >= Antenna.MaxPower)
			{
				return false;
			}
			if (requiredPower > Mobile.getPower(mobileClass))
			{
				return false;
			}
			return true;
		}

		private Double getAmbientInterference(Mobile m)
		{
			return AUSimulator.getOtherCellsInterferences(this, m);
		}

		private Double computeSIR(Double recievedPower, Mobile m)
		{
			//Double SIR = ((recievedPower) / (getAmbientInterference() + computeCellInterference(m) + recievedPower + _thermalNoise));
			return ((recievedPower) / (getAmbientInterference(m) + computeCellInterference(m) + recievedPower + _thermalNoise));
		}

		public double computeCellInterference(Mobile m)
		{
			//Debug.Assert(!CallingMobiles.ContainsKey(m));
			Debug.Assert(!Object.ReferenceEquals(CallingMobiles, null));

			Double interference = 0;
			foreach (Mobile mobile in CallingMobiles.Keys)
			{
				Double requiredPower = CallingMobiles[mobile].Item1;
				interference += frissPower(requiredPower, m);
			}
			return interference;
		}
	}
}
