using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
	public class Cell
	{
		private static readonly Int32 _thermalNoise = -90;
	
		private Int32 _frequency;

		private Double _cellInterference = 0;

		private readonly Double _umtsBandwidth = 3840000;


		private Antenna _antenna;
		public Antenna Antenna
		{
			get { return _antenna; }
		}

		// This list is representing 
		// ALL MOBILES CURRENTLY USING THIS CELL'S ANTENNA
		// with the power and the SIR consumed associated to free ressources
		private Dictionary<Mobile, Tuple<Double,Double>> _callingMobiles;
		public Dictionary<Mobile, Tuple<Double, Double>> CallingMobiles
		{
			get { return _callingMobiles; }
		}

		// This list is represent all pending mobiles, with remaining time associated
		private Dictionary<Mobile, Int32> _pendingMobiles;
		public Dictionary<Mobile, Int32> PendingMobiles
		{
			get { return _pendingMobiles; }
		}

		// array storing the UMTS codes under the form <Size, Number>
		private Dictionary<Int32, Int32> _codesArray;

		// To replace _codeArray short to mid-term;
		public UMTSCode Dummy;

		public Cell(Int32 frequency) : 
			this(frequency, 0, 0)
		{

		}

		// Constructors
		public Cell(Int32 frequency, Int32 x, Int32 y)
		{
			_antenna = new Antenna(x,y);
			_frequency = frequency;
			_pendingMobiles = new Dictionary<Mobile, Int32>();
			_callingMobiles = new Dictionary<Mobile,Tuple<Double,Double>>();
			_codesArray = new Dictionary<Int32, Int32>(){
				{1,1},
				{2,2},
				{3,3}
			};
		}

		// This method checks if the cell is able to accept the new call
		// if it is, then we wait for a code to be available
 		// and return it to the mobile
		public Int32 requestCall(Mobile m, Call.Type type)
		{
			Int32 code = (Int32) CallResult.FAILURE;

			Double receivedPower = frissPower(m);

			Double SIR = computeSIR(receivedPower, type);

			if (doAdmissionControl(receivedPower,SIR, type))
			{
				// The network is OK,
				// add mobile to list and wait for code
				_callingMobiles.Add(m, Tuple.Create<Double, Double>(receivedPower, SIR));
				

				if (_codesArray[1] > 0)
				{
					// There is some codes left, decrement it
					code = 1;
					_codesArray[1]--;
					// update SIR and power used
					_antenna.CurrentPower += receivedPower;
					_cellInterference += receivedPower;
				}
				else
				{
					// There is none, transmit a result to the simulator 
					// that it should check next iterations if there are some free codes 
					code = (Int32) CallResult.PENDING;
				}

			}
			return code;
		}

		private Double frissPower(Mobile m)
		{
			Double distance = Math.Sqrt(Math.Pow((m.X - _antenna.X), 2) + Math.Pow((m.Y - _antenna.Y),2));
			// Emitter and receptor Gains are 0 dB
			Double power = m.PE - (32.44 + 20 * Math.Log10(_frequency) + 20 * Math.Log10(distance));
			return power;
		}

		public Int32 endCall(Mobile m, Call.Type type)
		{
			return 0;
		}

		private Boolean doAdmissionControl(Double receivedPower, Double SIR, Call.Type type)
		{
			if (SIR < Call.getCallInfos()[type].Item1)
			{
				return false;
			}
			if (receivedPower > _antenna.MaxPower - 10)
			{
				return false;
			}
			if (receivedPower + _antenna.CurrentPower >= _antenna.MaxPower)
			{
				return false;
			}
			return true;
		}

		private Double getAmbientInterference()
		{
			return AUSimulator.getOtherCellsInterferences(this);
		}

		public Double getInterference()
		{
			return _cellInterference;
		}

		private Double computeSIR(Double recievedPower , Call.Type type)
		{
			Double SIR;
			Double SF = ((_umtsBandwidth) / (Call.getCallInfos()[type].Item2));
			SIR = SF * ((recievedPower) / (getAmbientInterference() + _cellInterference + _thermalNoise));
			return SIR;
		}

		private Boolean checkPower(Double recievedPower)
		{
			if (recievedPower > _antenna.MaxPower - 10)
			{
				return false;
			}
			if (recievedPower + _antenna.CurrentPower >= _antenna.MaxPower)
			{
				return false;
			}
			return true;
		}
	}
}
