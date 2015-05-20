using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using AdmissionCallSim.SimCore;

namespace AdmissionCallSim
{
    public class PhoneInfo
    {
        public int id { set; get; }
        public double x { set; get; }
        public double y { set; get; }
        public ObservableCollection<Call.Type> callType { set; get; }

    }
}
