using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class CashBank
    {
        internal int totalCount;
        internal int fivesCount;
        internal int tensCount;
        internal int twentiesCount;
        internal int fiftiesCount;
        internal int hundredsCount;
        internal int _moneyToDisburse;
        internal bool isEmpty;

        public CashBank()
        {
            totalCount = 3700;
            fivesCount = 20;
            tensCount = 20;
            twentiesCount = 20;
            fiftiesCount = 20;
            hundredsCount = 20;
            _moneyToDisburse = 0;
            isEmpty = false;
        }
    }
}
