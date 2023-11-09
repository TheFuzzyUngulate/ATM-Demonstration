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
        // handles functional buttons (0 = NIL, 1 = Cancel, 2 = Enter)
        internal List<Port> _port1;
        // handles number buttons, I guess...
        internal List<Port> _port2;
        // check whether keypad needs to work at the moment
        internal bool _keypadAvailable;

        public Keypad()
        {
            _keypadAvailable = false;

            _port1 = new List<Port>();
            for (var i = 0; i < 3; ++i)
                _port1.Add(new Port(0xFF00, false));

            _port2 = new List<Port>();
            for (var i = 0; i < 10; ++i)
                _port2.Add(new Port(0xFF00, false));
        }
    }
}
