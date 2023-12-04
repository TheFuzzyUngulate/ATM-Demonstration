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

        internal int _funcInput;
        internal int[] _numInputs;
        internal int _numInputCount;
        internal List<(int lastScan, int currentScan)> _numScanStatus;
        internal List<(int lastScan, int currentScan)> _funcScanStatus;
        internal bool _funcKeysAvailable, _numKeysAvailable;

        public Keypad()
        {
            port1 = new List<Port>();
            for (var i = 0; i < 3; ++i)
                port1.Add(new Port(0xFF00, 0));

            port2 = new List<Port>();
            for (var i = 0; i < 10; ++i)
                port2.Add(new Port(0xFF00, 0));

            _numInputs = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            _numInputCount = 0;
            _numScanStatus = new();
            for (int i = 0; i < 10; ++i)
                _numScanStatus.Add((0, 0));
            _numKeysAvailable = true;

            _funcInput = 0;
            _funcScanStatus = new() { (0, 0), (0, 0), (0, 0) };
            _funcKeysAvailable = true;
        }
    }
}
