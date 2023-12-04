using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.PeripheralDevices
{
    internal class Monitor
    {
        public string displayText;

        public string timeText;  

        internal Int32 monitorAddr;

        internal Monitor()
        {
            monitorAddr = 0xFF03;
        }

       public string getDisplayText()
        {
            return displayText;
        }
        public void setDisplayText(string value)
        {
            if (displayText != value)
            {
                System.Diagnostics.Debug.WriteLine("Set display text to: " + value);
                displayText = value;
                OnPropertyChanged(displayText);
            }
        }

        public string getTimeText()
        {
            return timeText;
        }
        public void setTimeText(string value)
        {
            if (timeText != value)
            {
                timeText = value;
                OnPropertyChanged(timeText);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
