using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.Utilities
{
    internal class Port
    {
        internal ushort _address;
        internal Queue<byte> _data;
        internal bool _isInput;
        internal bool _status;
        internal delegate void PortEvent(object src, EventArgs args);
        internal event PortEvent? BytesSent;

        public Port(ushort addr, bool isInput) {
            _address = addr;
            _status = true;
            _isInput = isInput;
            _data = new Queue<byte>();
        }

        protected virtual void OnBytesSent(EventArgs args) 
        {
            if (BytesSent != null)
            {
                BytesSent(this, args);
            }
        }

        public int GetLength()
        {
            return _data.Count;
        }

        public void Send(byte data)
        {
            if (_data.Count > 0)
                _data.Clear();

            _data.Enqueue(data);
            OnBytesSent(EventArgs.Empty);
        }

        public void Send(int data)
        {
            if (_data.Count > 0)
                _data.Clear();

            _data.Enqueue((byte)(data >> 24));
            _data.Enqueue((byte)(data >> 16));
            _data.Enqueue((byte)(data >> 8));
            _data.Enqueue((byte)(data));
            OnBytesSent(EventArgs.Empty);
        }

        public void Send(string data)
        {
            if (_data.Count > 0) 
                _data.Clear();

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < bytes.Length; ++i)
            {
                _data.Enqueue(bytes[i]);
            }
            OnBytesSent(EventArgs.Empty);
        }

        public byte RecvBytes()
        {
            if (_data.Count > 0)
            {
                byte b = _data.Dequeue();
                return b;
            }
            else return 0;
        }

        public int RecvInteger()
        {
            if (_data.Count > 0)
            {
                int i = 0;
                int retInteger = 0;
                while (i < 4 && i < _data.Count)
                {
                    var k = (_data.Count > 4) ? 3 : _data.Count - 1;
                    retInteger += (_data.Dequeue() << ((k - i) * 8));
                }
                return retInteger;
            }
            else return 0;
        }

        public string RecvString()
        {
            if (_data.Count > 0)
            {
                byte[] bytes = _data.ToArray();
                _data.Clear();
                return Encoding.UTF8.GetString(bytes);
            }
            else return "";
        }
    }
}
