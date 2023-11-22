using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.PeripheralDevices
{
    internal class Monitor
    {
        // port to send text
        internal Port _port1;
        // port to display amount of money
        internal Port _port2;
        // port to send warning alerts
        internal Port _port3;

        public Monitor()
        {
            _port1 = new Port(0xFD20, true);
            _port2 = new Port(0xFD30, true);
            _port3 = new Port(0xFDFF, true);
        }
    }
}
