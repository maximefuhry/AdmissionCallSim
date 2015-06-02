using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
    public class AUSimulator
    {
		private static List<Cell> _cells;

		// List for waiting mobiles, with timeout value
		private static Dictionary<Mobile, Int32> _pendingMobiles;

		private static Int32 _codeWaitTimeout;

		private static List<Mobile> _callingMobiles;

		private static List<Mobile> _idleMobiles;

		private static AUSimulator _instance;
		
		private static readonly Double _callingThresold = 0.2;

		public static Boolean Running { get; set; }

		private AUSimulator()
		{
			_cells = new List<Cell>();
			_callingMobiles = new List<Mobile>();
			_idleMobiles = new List<Mobile>();
			_pendingMobiles = new Dictionary<Mobile, Int32>();
			_codeWaitTimeout = 100;
			Running = true;
		}

		public static AUSimulator getInstance()
		{
			if (_instance == null)
			{
				_instance = new AUSimulator();
			}
			return _instance;
		}

		public void addMobile(Mobile m)
		{
			_idleMobiles.Add(m);
			setNearestCell(m);
		}

		public void removeMobile(Mobile m)
		{
			if (_callingMobiles.Contains(m) || _pendingMobiles.ContainsKey(m))
			{
				throw new Exception("Le mobile ne peut pas être supprimé en cours d'appel");
			}
			else if (_idleMobiles.Contains(m))
			{
				_idleMobiles.Remove(m);
			}
		}

		public void addCell(Cell c)
		{
			_cells.Add(c);
		}

		public void removeCell(Cell c)
		{
			if (c.CallingMobiles.Count != 0)
			{
				throw new Exception("Cell must be unused before deletion");
			}
			_cells.Remove(c);
		}

		private void runCalls(){
			foreach(Mobile m in _callingMobiles){
				m.runCall();
				if (m.CallLength == 0)
				{
					m.endCall();
					_callingMobiles.Remove(m);
					_idleMobiles.Add(m);
				}
			}
		}

		public static Double getOtherCellsInterferences(Cell calling, Mobile m){
			//Debug.Assert(!Object.ReferenceEquals(_instance, null));
			//Debug.Assert(!Object.ReferenceEquals(_cells, null));

			Double interference = 0;
			foreach (Cell cell in _cells)
			{
				if(cell != calling){
					interference += cell.computeCellInterference(m);
				}
			}
			return interference;
		}

		private void setNearestCell(Mobile mobile)
		{
			Double distance = Double.MaxValue;
			Double minDistance = distance;
			Cell nearestCell = null;
			foreach(Cell cell in _cells){
				distance = Math.Sqrt(Math.Pow((mobile.X - cell.Antenna.X), 2)+Math.Pow(mobile.Y - cell.Antenna.Y, 2));
				if (distance < minDistance){
					minDistance = distance;
					nearestCell = cell;
				}
			}
			mobile.NearestCell = nearestCell;
		}

		private void provokeRandomCalls()
		{
			Random RNG = new Random(DateTime.Now.Millisecond);
			foreach (Mobile m in _idleMobiles)
			{
				if (RNG.NextDouble() <= _callingThresold)
				{
					Int32 rndCallLength = RNG.Next(_codeWaitTimeout - (_codeWaitTimeout / 4), _codeWaitTimeout + (_codeWaitTimeout / 4));
					CallResult result = m.startCall((Call.Type) RNG.Next(1, 4), rndCallLength);
					switch (result)
					{
						case CallResult.PENDING:
							_idleMobiles.Remove(m);
							_pendingMobiles.Add(m, _codeWaitTimeout);
							break;
						case CallResult.SUCCESS:
							_idleMobiles.Remove(m);
							_callingMobiles.Add(m);
							break;
						case CallResult.FAILURE:
						default:
							break;
					}
				}
			}
		}

		// Method used to check every step
		// of the simulation the pending mobiles
		private void updatePending()
		{
			//Dictionary<Mobile, Int32> pendingmobiles = c.PendingMobiles;
			foreach (Mobile m in _pendingMobiles.Keys)
			{
				Int32 position;
				Cell c = m.NearestCell;

				Debug.Assert(c.PendingMobiles.ContainsKey(m));

				Int32 codeLen = Call.getCallInfos()[m.Type].Item3;

				if (c.UMTSCodes.requireCode(codeLen, out position))
				{
					c.CallingMobiles.Add(m, Tuple.Create<Double, Int32, Int32>(c.PendingMobiles[m], codeLen, position));
					c.PendingMobiles.Remove(m);
					_pendingMobiles.Remove(m);
					_callingMobiles.Add(m);
				}
				else
				{
					_pendingMobiles[m]--;
					if (_pendingMobiles[m] <= 0)
					{
						// Time-out expired
						c.PendingMobiles.Remove(m);

						// Set call length to 0 and close connection
						m.CallLength = 0;
						m.endCall();
					}
				}
			}
		}

		public static void startMobileCall(Mobile m)
		{
			Debug.Assert(_idleMobiles.Contains(m));

			Random RNG = new Random(DateTime.Now.Millisecond);
			Int32 callLen = RNG.Next(_codeWaitTimeout - (_codeWaitTimeout / 4), _codeWaitTimeout + (_codeWaitTimeout / 4));
			CallResult res = m.startCall((Call.Type)RNG.Next(1, 4), callLen);
			switch (res)
			{
				case CallResult.PENDING:
					_idleMobiles.Remove(m);
					_pendingMobiles.Add(m, _codeWaitTimeout);
					break;
				case CallResult.SUCCESS:
					_idleMobiles.Remove(m);
					_callingMobiles.Add(m);
					break;
				case CallResult.FAILURE:
				default:
					break;

			}
		}

		public void run()
		{

			while(Running)
			{
				_instance.runCalls();
				_instance.updatePending();
				_instance.provokeRandomCalls();
			}
			
			// Model safety : all calling mobiles (triggered by simulator or GUI) 
			// should end call before thread returns ?????

			//foreach (Mobile m in _callingMobiles)
			//{
			//	m.CallLength = 0;
			//	m.endCall();
			//	_callingMobiles.Remove(m);
			//	_idleMobiles.Add(m);
			//}
		}


		public static double computeDistance(Mobile currentMobile, Cell cell)
		{
			Debug.Assert(_cells.Contains(cell));
			Debug.Assert(!Object.ReferenceEquals(cell.Antenna, null));

			//Double distance = Math.Sqrt(Math.Pow(currentMobile.X - antenna.X, 2) + Math.Pow(currentMobile.Y-antenna.Y, 2));
			return Math.Sqrt(Math.Pow((currentMobile.X - cell.Antenna.X), 2) + Math.Pow((currentMobile.Y - cell.Y), 2));
		}
	}
}
