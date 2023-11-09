using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class CashBank
    {
        internal int c_total;
        internal int c_5bills;
        internal int c_10bills;
        internal int c_20bills;
        internal int c_50bills;
        internal int c_100bills;
        internal int _moneyToDisburse;

        public CashBank() {
            c_total = 3700;
            c_5bills = 20;
            c_10bills = 20;
            c_20bills = 20;
            c_50bills = 20;
            c_100bills = 20;
            _moneyToDisburse = 0;
        }
    }
}
