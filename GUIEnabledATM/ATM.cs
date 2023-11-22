using System;
using GUIEnabledATM.Utilities;
using System.Collections.Generic;
using GUIEnabledATM.InternalDevices;
using GUIEnabledATM.PeripheralDevices;

namespace GUIEnabledATM
{
    internal class ATM
    {
        internal CardScanner cardScanner;
        internal Keypad keypad;
        internal GUIEnabledATM.PeripheralDevices.Monitor monitor;
        internal AccountDatabase accountDatabase;

        internal CashBank cashBank;
        internal CashDisburser disburser;
        internal SystemClock clock;

        internal Processor SCB;

        internal bool SystemInitial;
        internal bool SystemShutdown;
        readonly System.Timers.Timer s_timer;

        internal bool isSafeToShutdown;

        public ATM()
        {
            cardScanner = new();
            keypad = new Keypad();
            monitor = new();
            accountDatabase = new();
            cashBank = new CashBank();
            disburser = new CashDisburser();
            clock = new SystemClock();

            SCB = new Processor();
            SystemInitial = true;
            SystemShutdown = false;

            s_timer = new System.Timers.Timer { Interval = 100 };
            s_timer.Start();
            isSafeToShutdown = false;
        }

        internal void SystemFailure()
        {
            // indicate system shutdown
            SystemShutdown = true;
            System.Diagnostics.Debug.WriteLine("Shutting down...");
            // wait for process dispatch to know, just to be safe
            while (!isSafeToShutdown) ;

            // show error message
            monitor._port1.Send("System failed. Shutting down...");
            monitor._port2.Send("");
            monitor._port3.Send("");

            // wait for 3 seconds
            SCB._waitTimer = (true, 3);
            while (SCB._waitTimer.remTime > 0) ;

            // close timers
            clock._timers.Clear();

            // dismiss cyclical interrupts
            s_timer.Close();

            // reset all ports
            monitor._port1.Send("");
            keypad._port1.ForEach((Port x) => { x.Send(""); });
            keypad._port2.ForEach((Port x) => { x.Send(""); });
            cardScanner._port.Send("");
        }

        protected virtual void SystemClock(object? src, System.Timers.ElapsedEventArgs e)
        {
            clock._100msCounter++;
            if (clock._100msCounter == 10)
            {
                // reset time
                clock._100msCounter = 0;
                clock._absTime++;
                clock.IncCurrentTime();

                // check timers
                if (SCB._waitTimer.isOn)
                {
                    if (SCB._waitTimer.remTime > 0)
                    {
                        SCB._waitTimer = (true, SCB._waitTimer.remTime - 1);
                    }
                    else SCB._waitTimer.isOn = false;
                }

                // set current time
                if (clock._currentTime.hours == 60)
                {
                    clock.ResetCurrentTime();
                    clock._absTime = 0;
                }
            }
        }

        private void ProcessDispatch()
        {
            switch (SCB._procState)
            {
                case 0:
                    SCB.Welcome(monitor, cardScanner);
                    break;
                case 1:
                    SCB.CheckPIN(accountDatabase, monitor);
                    break;
                case 2:
                    SCB.CheckAmount(monitor);
                    break;
                case 3:
                    SCB.VerifyAccBalance(accountDatabase, monitor);
                    break;
                case 4:
                    SCB.VerifyCashAvailability(accountDatabase, cashBank, monitor);
                    break;
                case 5:
                    SCB.DisburseCash(disburser, cashBank, monitor);
                    break;
                case 6:
                    SCB.EjectCard(cardScanner, monitor);
                    break;
            }
        }

        private void EventCapture(object? src, System.Timers.ElapsedEventArgs e)
        {
            if (!cardScanner._cardInserted)
            {
                int uID = cardScanner._port.RecvInteger();
                if (uID > 0)
                {
                    SCB._currentUserID = uID;
                    cardScanner._cardInserted = true;
                }
            }

            if (SCB._numInputCount < SCB._numInputs.Length)
            {
                for (int i = 0; i < SCB._numScanStatus.Count; ++i)
                {
                    SCB._numScanStatus[i] = (SCB._numScanStatus[i].currentScan, keypad._port2[i].RecvBytes());

                    if (SCB._numScanStatus[i].currentScan != SCB._numScanStatus[i].lastScan
                        && SCB._numScanStatus[i].currentScan == 1)
                    {
                        SCB._numScanStatus[i] = (SCB._numScanStatus[i].currentScan, SCB._numScanStatus[i].currentScan);
                        
                        if (SCB._numKeysAvailable)
                        {
                            SCB._numInputs[SCB._numInputCount] = i;
                            ++SCB._numInputCount;
                            System.Diagnostics.Debug.WriteLine("Number count is now " + SCB._numInputCount);

                            int str = 0;
                            for (int j = 0; j < SCB._numInputCount; ++j)
                                str = str * 10 + SCB._numInputs[j];
                            monitor._port2.Send(str);
                        }
                    }
                }
            }

            for (int i = 0; i < SCB._funcScanStatus.Count; ++i)
            {
                SCB._funcScanStatus[i] = (SCB._funcScanStatus[i].currentScan, keypad._port1[i].RecvBytes());

                if (SCB._funcScanStatus[i].currentScan != SCB._funcScanStatus[i].lastScan
                    && SCB._funcScanStatus[i].currentScan == 1)
                {
                    SCB._funcScanStatus[i] = (SCB._funcScanStatus[i].currentScan, SCB._funcScanStatus[i].currentScan);
                    System.Diagnostics.Debug.WriteLine(((i == 0) ? "CANCEL" : ((i == 1) ? "ENTER" : "CLEAR")) + " key entered.");


                    if (SCB._funcKeysAvailable)
                    {
                        SCB._funcInput = i + 1;
                    }
                }
            }
        }

        internal void SysDeployment()
        {
            if (SystemInitial)
            {
                s_timer.Elapsed += new System.Timers.ElapsedEventHandler(SystemClock);
                s_timer.Elapsed += new System.Timers.ElapsedEventHandler(EventCapture);
                
                while (!SystemShutdown)
                {
                    ProcessDispatch();
                } isSafeToShutdown = true;
            }
        }
    }
}
