using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
    public class AUSimulator
    {
		private List<Cell> _cells;

		// List for waiting mobiles, with timeout value
		private Dictionary<Mobile, Int32> _pendingMobiles;

		private readonly Int32 _codeWaitTimeout = 10;

		public List<Mobile> AllMobiles { get; private set; }

		private List<Mobile> _callingMobiles;

		public static List<Mobile> IdleMobiles;

		private static AUSimulator _instance;
		
		private static readonly Double _callingThresold = 0.2;

		public Boolean Running { get; set; }

		private AUSimulator()
		{
			_cells = new List<Cell>();
			AllMobiles = new List<Mobile>();
			_callingMobiles = new List<Mobile>();
			IdleMobiles = new List<Mobile>();
			_pendingMobiles = new Dictionary<Mobile, Int32>();
			//_codeWaitTimeout = 15;
			//Running = true;
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
			AllMobiles.Add(m);
			IdleMobiles.Add(m);
			setNearestCell(m);
		}

		public void removeMobile(Mobile m)
		{
			if (_callingMobiles.Contains(m) || _pendingMobiles.ContainsKey(m))
			{
				throw new InvalidOperationException("Le mobile ne peut pas être supprimé en cours d'appel");
			}
			else if (IdleMobiles.Contains(m))
			{
				AllMobiles.Remove(m);
				IdleMobiles.Remove(m);
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
				throw new InvalidOperationException("Cell must be unused before deletion");
			}
			_cells.Remove(c);
		}

		private void runCalls(){
			List<Mobile> toRemove = new List<Mobile>();
			foreach(Mobile m in _callingMobiles)
			{
				m.runCall();
				if (m.CallLength == 0)
				{
					m.endCall();
					toRemove.Add(m);
					IdleMobiles.Add(m);
				}
			}
			foreach (Mobile m in toRemove)
			{
				_callingMobiles.Remove(m);
			}
		}

		public static Double getOtherCellsInterferences(Cell calling, Mobile m){
			//Debug.Assert(!Object.ReferenceEquals(_instance, null));
			//Debug.Assert(!Object.ReferenceEquals(_cells, null));

			Double interference = 0;
			foreach (Cell cell in _instance._cells)
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
			foreach (Mobile m in IdleMobiles)
			{
				if (RNG.NextDouble() <= _callingThresold)
				{
					startMobileCall(m, (Call.Type) RNG.Next(1,4));					
				}
			}
		}

		// Method used to check every step
		// of the simulation the pending mobiles
		private void updatePending()
		{
			List<Mobile> toRemove = new List<Mobile>();
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
					toRemove.Add(m);
					_callingMobiles.Add(m);
				}
				else
				{
					_pendingMobiles[m]--;
					if (_pendingMobiles[m] <= 0)
					{
						// Time-out expired
						c.PendingMobiles.Remove(m);
						toRemove.Add(m);
						IdleMobiles.Add(m);

						// Set call length to 0 and close connection
						m.CallLength = 0;
						m.endCall(false);
					}
				}
			}
		}

		public static void startMobileCall(Mobile m, Call.Type service)
		{
			Debug.Assert(IdleMobiles.Contains(m));

			switch (m.startCall(service, new Random(DateTime.Now.Millisecond).Next(_instance._codeWaitTimeout - (_instance._codeWaitTimeout / 4), _instance._codeWaitTimeout + (_instance._codeWaitTimeout / 4))))
			{
				case CallResult.PENDING:
					IdleMobiles.Remove(m);
					_instance._pendingMobiles.Add(m, _instance._codeWaitTimeout);
					break;
				case CallResult.SUCCESS:
					IdleMobiles.Remove(m);
					_instance._callingMobiles.Add(m);
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
				runCalls();
				updatePending();
				//provokeRandomCalls();
				Thread.Sleep(5000);
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
			Debug.Assert(_instance._cells.Contains(cell));
			Debug.Assert(!Object.ReferenceEquals(cell.Antenna, null));

			//Double distance = Math.Sqrt(Math.Pow(currentMobile.X - antenna.X, 2) + Math.Pow(currentMobile.Y-antenna.Y, 2));
			return Math.Sqrt(Math.Pow((currentMobile.X - cell.Antenna.X), 2) + Math.Pow((currentMobile.Y - cell.Antenna.Y), 2));
		}
	}
}
