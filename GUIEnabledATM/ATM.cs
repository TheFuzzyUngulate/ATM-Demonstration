using System;
using GUIEnabledATM.Utilities;
using System.Collections.Generic;
using GUIEnabledATM.InternalDevices;
using GUIEnabledATM.PeripheralDevices;

namespace GUIEnabledATM
{
    internal struct DataScan
    {
        public int currentDataScan;
        public int lastDataScan;
    }
    internal struct Key
    {
        public int dataKey;
        public int funcKey;
        public int entry;
        public int currentAccount;
        public int withdrawCount;

    }
    internal class ATM
    {
        internal CardScanner scanner;
        internal Keypad keypad;
        internal Monitor monitor;
        internal CashBank bank;
        internal CashDisburser disburser;
        internal AccountDatabase accountDatabase;
        internal SystemClock clock;
        internal List<DataScan> currentDataScan = new List<DataScan>();
        internal bool currentFuncScan;
        internal bool lastFuncScan;

        internal ATM() { }

        public void SysInitial()
        {

        }

        public void EventCapture()
        {

        }
        public void SystemDispatch()
        {

        }

        public void SystemDeployment()
        {

        }

        public void CheckPin()
        {

        }

        public void CheckAmount(int amount)
        {

        }

        public void DisburseBills()
        {

        }

        public void EjectCard()
        {

        }

        public void InputWithdrawAmount()
        {

        }

        public void SystemClock()
        {

        }

        public void SystemFailure()
        {

        }

        public void VerifyBalance()
        {

        }

        public void VerifyBillsAvailability()
        {

        }
        public void Welcome()
        {

        }
    }
}
