using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class SystemClock
    {
        internal int _absTime;
        internal int _100msCounter;
        internal List<(bool isOn, int remTime)> _timers;
        internal (int hours, int minutes, int seconds) _currentTime;

        public SystemClock()
        {
            _absTime = 0;
            _timers = new();
            _100msCounter = 0;
            _currentTime = (0, 0, 0);
        }

        internal string GetCurrentTime()
        {
            return _currentTime.hours.ToString() + ":" + _currentTime.minutes.ToString();
        }

        internal void ResetCurrentTime()
        {
            _currentTime = (0, 0, 0);
        }

        internal void IncCurrentTime()
        {
            if (_currentTime.hours < 60)
            {
                _currentTime = (_currentTime.hours, _currentTime.minutes, _currentTime.seconds + 1);
                if (_currentTime.seconds == 60)
                {
                    _currentTime = (_currentTime.hours, _currentTime.minutes + 1, 0);
                }
                if (_currentTime.minutes == 60)
                {
                    _currentTime = (_currentTime.hours + 1, 0, 0);
                }
            }
        }
    }
}
