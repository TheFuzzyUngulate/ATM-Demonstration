using GUIEnabledATM.PeripheralDevices;
using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class Processor
    {
        internal int _procState;
        internal int _currentUserID;
        internal int _requestedAmount;
        internal int _currentPINAttempts;

        internal const int MAX_PIN_ATTEMPTS = 3;
        internal const int MAX_WITHDRAW_AMOUNT = 1000000;
        internal (bool isOn, int remTime) _waitTimer;

        internal int _funcInput;
        internal int[] _numInputs;
        internal int _numInputCount;
        internal List<(int lastScan, int currentScan)> _numScanStatus;
        internal List<(int lastScan, int currentScan)> _funcScanStatus;
        internal bool _funcKeysAvailable, _numKeysAvailable;

        public Processor()
        {
            // global variables
            _procState = 0;
            _currentUserID = 0;
            _requestedAmount = 0;
            _currentPINAttempts = 0;
            _waitTimer = (false, 0);

            _numInputs = new int[7] {0, 0, 0, 0, 0, 0, 0};
            _numInputCount = 0;
            _numScanStatus = new();
            for (int i = 0; i < 10; ++i)
                _numScanStatus.Add((0, 0));
            _numKeysAvailable = false;

            _funcInput = 0;
            _funcScanStatus = new() {(0, 0), (0, 0), (0, 0)};
            _funcKeysAvailable = false;
        }

        public void SystemFailure()
        {
            // do some stuff here
            // i'm not even sure what the hell goes here...
        }

        public void Welcome(PeripheralDevices.Monitor mn, CardScanner sc, Keypad key)
        {
            mn._port1.Send("Welcome. Please enter your card to begin");
            
            if (sc._cardInserted)
            {
                mn._port1.Send("Please enter your pin");
                _numKeysAvailable = true;
                _funcKeysAvailable = true;
                _procState = 1;
            }
        }

        public void CheckPIN(Keypad key, AccountDatabase db, PeripheralDevices.Monitor mn)
        {

            if (_funcInput > 0)
            {
                if (_funcInput == 1)
                {
                    mn._port3.Send("Transaction cancelled");
                    _procState = 6;
                }
                else if (_funcInput == 2)
                {
                    System.Diagnostics.Debug.WriteLine("Enter was.... entered...");
                    if (_numInputCount >= 4)
                    {
                        try
                        {
                            int in_pin = _numInputs[3]
                                       + _numInputs[2] * 10
                                       + _numInputs[1] * 100
                                       + _numInputs[0] * 1000;
                            System.Diagnostics.Debug.WriteLine("Attempting to write PIN: " + in_pin);

                            mn._port3.Send("");
                            db.Login(_currentUserID, in_pin);

                            System.Diagnostics.Debug.WriteLine("Going to state 3.");
                            mn._port1.Send("How much would you like to withdraw?");

                            _numInputCount = 0;
                            _procState = 2;

                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            mn._port1.Send("Unregistered card entered!");
                            _procState = 6;
                        }
                        catch (ArgumentException)
                        {
                            mn._port3.Send("Invalid PIN entered!");

                            _currentPINAttempts++;

                            if (_currentPINAttempts >= MAX_PIN_ATTEMPTS)
                            {
                                mn._port1.Send("No more retries.");
                                _currentPINAttempts = 0;
                                _procState = 6;
                            }
                            else
                            {
                                mn._port3.Send((MAX_PIN_ATTEMPTS - _currentPINAttempts) + " retries left.");
                            }
                        }
                    }
                    else
                    {
                        mn._port3.Send("Incomplete PIN");
                    }
                }

                for (int i = 0; i < _numInputs.Length; ++i)
                    _numInputs[i] = 0;
                _funcInput = 0;
            }
        }

        public void CheckAmount(Keypad key, Monitor mn)
        {
            if (_funcInput > 0)
            {
                if (_funcInput == 1)
                {
                    mn._port1.Send("Transaction cancelled");
                    _procState = 6;
                }
                else if (_funcInput == 2)
                {
                    if (_numInputCount > 0)
                    {
                        int reqAmount = 0;
                        for (var i = 0; i < _numInputCount; i++)
                        {
                            reqAmount += _numInputs[i] * (int)(Math.Pow(10, (_numInputCount - i - 1)));
                        }

                        if (reqAmount == 0)
                        {
                            mn._port3.Send("Can't withdraw less than 5 dollars");
                        }
                        else if (reqAmount > MAX_WITHDRAW_AMOUNT)
                        {
                            mn._port3.Send("Can't withdraw more than " + MAX_WITHDRAW_AMOUNT + " dollars");
                        }
                        else
                        {
                            mn._port3.Send("");
                            System.Diagnostics.Debug.WriteLine("Going to state 4...");

                            _numKeysAvailable = false;
                            _funcKeysAvailable = false;
                            _procState = 3;
                            _requestedAmount = reqAmount;
                        }
                    }
                    else
                    {
                        mn._port3.Send("Amount not specified");
                    }
                }

                for (int i = 0; i < _numInputs.Length; ++i)
                    _numInputs[i] = 0;
                _funcInput = 0;
            }
        }

        public void VerifyAccBalance(AccountDatabase db, Monitor mn)
        {
            var trueBal = db.GetBalance(_currentUserID);
            if (trueBal >= _requestedAmount)
            {
                System.Diagnostics.Debug.WriteLine("Going to state 5");
                _procState = 4;
            }
            else
            {
                mn._port3.Send("Not enough money inside account");
                _procState = 6;
            }
        }

        public void VerifyCashAvailability(CashBank bank, Monitor mn)
        {
            if (bank.c_total < _requestedAmount)
            {
                mn._port3.Send("Not enough cash in bank");
                _procState = 6;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Going to state 6");
                _procState = 5;
            }
        }

        public void DisburseCash(CashDisburser cdb, Monitor mn)
        {
            while (_requestedAmount >= 100)
            {
                cdb._bills.Item5++;
                _requestedAmount -= 100;
            }
            while (_requestedAmount >= 50)
            {
                cdb._bills.Item4++;
                _requestedAmount -= 50;
            }
            while (_requestedAmount >= 20)
            {
                cdb._bills.Item3++;
                _requestedAmount -= 20;
            }
            while (_requestedAmount >= 10)
            {
                cdb._bills.Item2++;
                _requestedAmount -= 10;
            }
            while (_requestedAmount >= 5)
            {
                cdb._bills.Item1++;
                _requestedAmount -= 5;
            }
            mn._port1.Send("Disbursing cash...");
            cdb.Disburse();
            mn._port1.Send("Cash disbursed");
            _procState = 6;
        }

        public void EjectCard(CardScanner sc, Monitor mn)
        {
            _currentUserID = 0;
            sc._cardInserted = false;

            if (_waitTimer.isOn)
            {
                if (_waitTimer.remTime == 0)
                {
                    _procState = 0;
                    _waitTimer.isOn = false;
                    mn._port3.Send("");
                }
            }
            else
            {
                mn._port1.Send("Card ejected");
                _waitTimer = (true, 6);
            }
        }
    }


}
