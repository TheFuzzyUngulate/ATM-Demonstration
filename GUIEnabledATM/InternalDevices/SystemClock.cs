using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class SystemClock
    {
        internal int tickCounter;
        internal int _100msCounter; 
        internal int relativeCounter;
        internal (int hours, int minutes, int seconds) currentTime;

        public SystemClock()
        {
            updateCurrentTime();
        }

        internal string GetCurrentTime()
        {
            return currentTime.hours.ToString() + ":" + currentTime.minutes.ToString();
        }

        internal void ResetCurrentTime()
        {
            currentTime = (0, 0, 0);
        }

        internal void updateCurrentTime()
        {

            currentTime.hours = DateTime.Now.Hour;
            currentTime.minutes = DateTime.Now.Minute;
            currentTime.seconds = DateTime.Now.Second;
        }
    }
}
