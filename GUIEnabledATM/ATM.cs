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

        //these 4 variables and the above structs are meant to represent whats in the SCB
        internal List<DataScan> currentDataScan = new List<DataScan>();
        internal bool currentFuncScan;
        internal bool lastFuncScan;
        internal List <Key> keyList = new List<Key>();
        public int currentAccount;
        public int withdrawCount;

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

        public bool DisburseBills(int amount, int denom)
        {
            if(disburser.status == false)
            {
                disburser.status = true;
                if(denom == 0)
                {
                    bank.fivesCount = bank.fivesCount - amount;
                }
                if(denom == 1)
                {
                    bank.tensCount = bank.tensCount - amount;
                }
                if(denom == 2)
                {
                    bank.twentiesCount = bank.twentiesCount - amount;
                }
                if (denom == 3)
                {
                    bank.fiftiesCount = bank.fiftiesCount - amount;
                }
                if(denom == 4)
                {
                    bank.hundredsCount = bank.hundredsCount - amount;
                }
                disburser.status = false;
                disburser.billsDisbursed = true;
                return true;
            }
            else
            {
                disburser.billsDisbursed = false;
                return false;
            }
            
        }

        public void EjectCard()
        {
            scanner.status = false;
            Welcome();
        }

        public void InputWithdrawAmount()
        {
            monitor.displayText = "Please select your amount";
            bool isEmpty = bank.isEmpty;
            if (!isEmpty)
            {
                //this part does not translate nice and I think we're going to have to switch from whats in lab 3
                //disburser.denom = SCB.entry
                //disburser.count = SCB.withdrawCount
                VerifyBalance();
            }
            else
            {
                monitor.displayText = "ATM has no cash";
                Welcome() ;
            }
        }

        //not sure if we need to implement this one since we are simulating the clock using the system clock
        public void SystemClock()
        {

        }

        public void SystemFailure()
        {
            monitor.displayText = "ERROR: System Failure";
            disburser.status = true;
            scanner.status = false;
            sysDatabase.maxAllowableWithdraw = 0;

        }

        public void VerifyBalance()
        {
            if(sysDatabase.getBalance(currentAccount) >= withdrawCount)
            {
                if(sysDatabase.getMaxWithdraw(currentAccount) >= withdrawCount)
                {
                    VerifyBillsAvailability(withdrawCount);
                }
                else
                {
                    monitor.displayText = "Withdrawl amount Too large";
                    EjectCard();
                }
            }
            else
            {
                monitor.displayText = "Insufficient funds";
                EjectCard();
            }
        }


        //this function is modified from whats in lab 3 to properly calculate the number of each denom needed
        public void VerifyBillsAvailability(int amount)
        {

            if(amount >= bank.totalCount)
            {
                //this section calculates the number of each denom to dispense
                int[] denomination = { 100, 50, 20, 10, 5 };
                Dictionary<int, int> denomAmount = new Dictionary<int, int>();
                foreach(int denom in denomination)
                {
                    int count = amount/denom;

                    if(count > 0 && count <=bank.getCount(denom))
                    {
                        denomAmount[denom] = count;
                        amount %= denom;
                    }
                }

                if(amount == 0)
                {
                    foreach(var denom in denomAmount) 
                    {
                        if(denom.Value > 0)
                        {
                            bool success = false;
                            do
                            {
                                success = DisburseBills(denom.Value, denom.Key);
                            }
                            while (!success);
                        }
                    }
                }
                
            }
            else
            {
                monitor.displayText = "Insufficient funds";
                EjectCard();
            }
            monitor.displayText = "Insufficient funds";
            EjectCard();
        }
        public void Welcome()
        {
            monitor.displayText = "Welcome. Please enter your card to begin";
            monitor.timeText = clock.GetCurrentTime();
            if(scanner.status == true )
            {
                monitor.displayText = "Please enter your pin";
            }
        }
    }
}
