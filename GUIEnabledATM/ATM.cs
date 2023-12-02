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
        internal AccountDatabase sysDatabase;
        internal SystemClock clock;
        internal List<DataScan> currentDataScan = new List<DataScan>();
        internal bool currentFuncScan;
        internal bool lastFuncScan;
        internal List <Key> keyList = new List<Key>();

        internal ATM() { }


        //the following block might need to go into another class
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
        //end of block

        public void CheckPin(int pinNum)
        {
            int i = pinNum;
            string cardNum = scanner.cardNum;
            bool pinCorrect = sysDatabase.checkPinIsCorrect(cardNum, i);
            bool accountActive = sysDatabase.accountActive(cardNum);
            if (accountActive && pinCorrect) {
                monitor.displayText = "PIN OK";
                InputWithdrawAmount();
            }
            else
            {
                monitor.displayText = "PIN OK";
                Welcome();
            }
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
            monitor.displayText = "Please select your amount";
            bool isEmpty = bank.isEmpty;
            if (!isEmpty)
            {
                disburser.denom = en
            }
            else
            {
                monitor.displayText = "ATM has no cash";
                Welcome() ;
            }
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
            monitor.displayText = "Welcome. Please etner your card to begin";
            monitor.timeText = clock.GetCurrentTime();
            if(scanner.status == true )
            {
                monitor.displayText = "Please enter your pin";
            }
        }
    }
}
