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
        public string displayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                OnPropertyChanged(displayText);
            }
        }
        public string timeText
        {
            get { return timeText; }
            set
            {
                timeText = value;
                OnPropertyChanged(timeText);
            }
        }
        internal Int32 monitorAddr;

        internal Monitor()
        {
            monitorAddr = 0xFF03;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
