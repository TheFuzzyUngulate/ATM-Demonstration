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

        internal Port _port;
        internal bool _cardInserted;
        internal int _recvCode;

        public CardScanner() 
        {
            _recvCode = 0;
            _cardInserted = false;
            _port = new Port(0xFF00, false);
            //_port.BytesSent += CardScanner_If_Bytes_Sent;
        }

        /* private void CardScanner_If_Bytes_Sent(object src, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Card scanned.");
            _cardInserted = true;
        } */
    }
}
