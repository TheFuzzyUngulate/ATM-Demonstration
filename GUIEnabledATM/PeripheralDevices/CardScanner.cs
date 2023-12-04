using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.PeripheralDevices
{
    internal class CardScanner
    {
        internal bool status;
        internal Port port;
        internal string cardNum;
        internal string cardHolder;

        public CardScanner()
        {
            cardNum = "0";
        }
       
    }
}
