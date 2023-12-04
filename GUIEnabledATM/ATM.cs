using System;
using GUIEnabledATM.Utilities;
using System.Collections.Generic;
using GUIEnabledATM.InternalDevices;
using GUIEnabledATM.PeripheralDevices;
using System.Diagnostics;

namespace GUIEnabledATM
{
    internal struct DataScan
    {
        public int currentDataScan;
        public int lastDataScan;
    }
    internal struct Keys
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

        //these 4 variables and the above structs are meant to represent whats in the keypad
        internal List<DataScan> currentDataScan = new List<DataScan>();
        internal bool currentFuncScan;
        internal bool lastFuncScan;
        internal List <Keys> keyList = new List<Keys>();
        public int currentAccount;
        public int withdrawCount;
        public int withdrawDenom;
        public int withdrawNum;


        //missing from keypad in lab 3 but probably should be included
        internal bool SystemInitial;
        internal bool SystemShutdown;
        readonly System.Timers.Timer s_timer;

        internal bool isSafeToShutdown;

        internal int _procState;

        internal ATM() {
            scanner = new();
            keypad = new Keypad();
            monitor = new();
            sysDatabase = new();
            bank = new CashBank();
            disburser = new CashDisburser();
            clock = new SystemClock();

            //SCB = new Processor();
            SystemInitial = true;
            SystemShutdown = false;

            s_timer = new System.Timers.Timer { Interval = 100 };
            s_timer.Start();
            isSafeToShutdown = false;
        }


        //the following block might need to go into another class
        

        public void EventCapture(object? src, System.Timers.ElapsedEventArgs e)
        {
            monitor.timeText = clock.GetCurrentTime();
            if (!scanner.status)
            {
                int uID = int.Parse(scanner.cardNum);
                if (uID > 0)
                {
                    currentAccount = uID;
                    scanner.status = true;
                }
            }
            if (keypad._numInputCount < keypad._numInputs.Length)
            {
                for (int i = 0; i < keypad._numScanStatus.Count; ++i)
                {
                    keypad._numScanStatus[i] = (keypad._numScanStatus[i].currentScan, keypad.port2[i]._input);

                    if (keypad._numScanStatus[i].currentScan != keypad._numScanStatus[i].lastScan
                        && keypad._numScanStatus[i].currentScan == 1)
                    {
                        keypad._numScanStatus[i] = (keypad._numScanStatus[i].currentScan, keypad._numScanStatus[i].currentScan);

                        if (keypad._numKeysAvailable)
                        {
                            keypad._numInputs[keypad._numInputCount] = i;
                            ++keypad._numInputCount;
                            System.Diagnostics.Debug.WriteLine("Number count is now " + keypad._numInputCount);

                            int str = 0;
                            for (int j = 0; j < keypad._numInputCount; ++j)
                                str = str * 10 + keypad._numInputs[j];
                            //monitor._port2.Send(str);
                        }
                    }
                }
            }

            for (int i = 0; i < keypad._funcScanStatus.Count; ++i)
            {
                keypad._funcScanStatus[i] = (keypad._funcScanStatus[i].currentScan, keypad.port1[i]._input);

                if (keypad._funcScanStatus[i].currentScan != keypad._funcScanStatus[i].lastScan
                    && keypad._funcScanStatus[i].currentScan == 1)
                {
                    keypad._funcScanStatus[i] = (keypad._funcScanStatus[i].currentScan, keypad._funcScanStatus[i].currentScan);
                    System.Diagnostics.Debug.WriteLine(((i == 0) ? "CANCEL" : ((i == 1) ? "ENTER" : "CLEAR")) + " key entered.");


                    if (keypad._funcKeysAvailable)
                    {
                        keypad._funcInput = i + 1;
                    }
                }
            }
        }
        public void SystemDispatch()
        {
            switch (_procState)
            {
                case 0:
                    Welcome();
                    break;
                case 1:
                    if (keypad._funcInput == 2)
                    {
                        int in_pin = keypad._numInputs[3]
                                           + keypad._numInputs[2] * 10
                                           + keypad._numInputs[1] * 100
                                           + keypad._numInputs[0] * 1000;
                        CheckPin(in_pin);
                    }
                    break;
                case 2:
                    InputWithdrawAmount();
                    break;
                case 3:
                    VerifyBalance();
                    break;
                case 4:
                    VerifyBillsAvailability(withdrawCount);
                    break;
                case 5:
                    DisburseBills(withdrawNum,withdrawDenom);
                    break;
                case 6:
                    EjectCard();
                    break;
            }
        }

        public void SysDeployment()
        {
            if (SystemInitial)
            {
                s_timer.Elapsed += new System.Timers.ElapsedEventHandler(EventCapture);

                while (!SystemShutdown)
                {
                    SystemDispatch();
                }

                isSafeToShutdown = true;
            }
        }
        //end of block

        public void CheckPin(int pinNum)
        {
            int i = pinNum;
            string cardNum = scanner.cardNum;
            bool pinCorrect = sysDatabase.checkPinIsCorrect(cardNum, i);
            bool accountActive = sysDatabase.accountActive(cardNum);
            if (accountActive && pinCorrect) {
                monitor.setDisplayText("PIN OK");
                InputWithdrawAmount();
            }
            else
            {
                monitor.setDisplayText("PIN NOT OK");
                _procState = 0;
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
                _procState = 4;
                return true;
            }
            else
            {
                disburser.billsDisbursed = false;
                _procState = 4;
                return false;
            }
            
        }

        public void EjectCard()
        {
            scanner.status = false;
            _procState = 0;
        }

        public void InputWithdrawAmount()
        {
            monitor.setDisplayText("Please select your amount");
            bool isEmpty = bank.isEmpty;
            if (!isEmpty)
            {
                //this part does not translate nice and I think we're going to have to switch from whats in lab 3
                //disburser.denom = keypad.entry
                //disburser.count = keypad.withdrawCount
                if (keypad._funcInput == 2)
                {
                    if (keypad._numInputCount > 0)
                    {
                        int reqAmount = 0;
                        for (var i = 0; i < keypad._numInputCount; i++)
                        {
                            reqAmount += keypad._numInputs[i] * (int)(Math.Pow(10, (keypad._numInputCount - i - 1)));
                        }

                        if (reqAmount == 0)
                        {
                            monitor.setDisplayText("Can't withdraw less than 5 dollars");
                        }
                        else
                        {
                            monitor.setDisplayText("");


                            keypad._numKeysAvailable = false;
                            keypad._funcKeysAvailable = false;
                            withdrawCount = reqAmount;
                            _procState = 3;

                        }
                    }
                    else
                    {
                        monitor.setDisplayText("Amount not specified");
                    }

                }
                else
                {
                    monitor.displayText = "ATM has no cash";
                    _procState = 0;
                }
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
                    _procState = 4;
                }
                else
                {
                    monitor.setDisplayText("Withdrawl amount Too large");
                    _procState = 6;
                }
            }
            else
            {
                monitor.setDisplayText("Insufficient funds");
                _procState = 6;
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
                                withdrawDenom = denom.Value;
                                withdrawNum = denom.Key;
                                _procState = 5;
                            }
                            while (!success);
                        }
                    }
                }
                _procState = 6;
                
            }
            else
            {
                monitor.setDisplayText("Insufficient funds");
                _procState = 6;
            }
            monitor.setDisplayText("Insufficient funds");
            _procState = 6;
        }
        public void Welcome()
        {
            monitor.setDisplayText("Welcome. Please enter your card to begin");
            monitor.setTimeText(clock.GetCurrentTime());
            if(scanner.status == true )
            {
                monitor.setDisplayText( "Please enter your pin");
            }
        }
    }
}
