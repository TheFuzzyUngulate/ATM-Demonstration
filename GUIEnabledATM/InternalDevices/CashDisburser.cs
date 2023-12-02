using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class CashDisburser
    {
        internal bool status;
        internal int denom;
        internal int amount;
        internal Int32 disburserAddr;
        internal bool billsDisbursed;

        internal CashDisburser()
        {
            disburserAddr = 0xFF05;
        }
    }
}
