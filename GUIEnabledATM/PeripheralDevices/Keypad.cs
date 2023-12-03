using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.PeripheralDevices
{
    internal class Keypad
    {
        internal List<Port> port1;
        // handles number buttons, I guess...
        internal List<Port> port2;

        public Keypad()
        {
            port1 = new List<Port>();
            for (var i = 0; i < 3; ++i)
                port1.Add(new Port(0xFF00, i));

            port2 = new List<Port>();
            for (var i = 0; i < 10; ++i)
                port2.Add(new Port(0xFF00, i));
        }
    }
}
