﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.Utilities
{
    internal class Port
    {
        internal ushort _address;
        internal int _input;

        public Port(ushort addr, int value)
        {
            _address = addr;
            _input = value;
        }
    }
}
