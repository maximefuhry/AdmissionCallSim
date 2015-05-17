using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionCallSim.SimCore
{
    class AUSimulator
    {
		private static List<Cell> _cells;

		// List for waiting mobiles, with timeout value
		private static Dictionary<Mobile, Int32> _pendingMobiles;

		private static Int32 _codeWaitTimeout = 100;

		private static List<Mobile> _callingMobiles;

		private static List<Mobile> _idleMobiles;

		private static AUSimulator _instance = null;

		private AUSimulator()
		{
			_cells = new List<Cell>();
			_callingMobiles = new List<Mobile>();
			_idleMobiles = new List<Mobile>();
			_pendingMobiles = new Dictionary<Mobile, int>();

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
			if (_callingMobiles.Contains(m))
			{
				throw new Exception("Le mobile ne peut pas être supprimé en cours d'appel");
			}
			if (_idleMobiles.Contains(m))
			{
				_idleMobiles.Remove(m);
			}
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

		public static Double getOtherCellsInterferences(Cell calling){
			Double interference = 0;
			//foreach (Cell cell in _cells)
			//{
			//	if(cell != calling){
			//		interference += cell.getInterference();
			//	}
			//}
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

		// Method used to check every step
		// of the simulation the pending mobiles
		private void updateCodes()
		{
			foreach (Cell c in _cells)
			{
				Dictionary<Mobile, Int32> pendingmobiles = c.PendingMobiles;
				foreach (Mobile m in pendingmobiles.Keys)
				{
					Int32 position;
					Debug.Assert(c.CallingMobiles.ContainsKey(m));
					Debug.Assert(_idleMobiles.Contains(m));

					if (c.Dummy.requireCode(Call.getCallInfos()[m.Type].Item3, out position))
					{
						// TODO update Cell interference with current mobile required power.
						c.PendingMobiles.Remove(m);
						_idleMobiles.Remove(m);
						_callingMobiles.Add(m);
					}
					else
					{
						c.PendingMobiles[m]--;
						if (c.PendingMobiles[m] <= 0)
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
		}
    }
}
