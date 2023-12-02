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
        internal string displayText;
        internal string timeText;
        internal Int32 monitorAddr;

        internal Monitor()
        {
            monitorAddr = 0xFF03;
        }
    }
}
