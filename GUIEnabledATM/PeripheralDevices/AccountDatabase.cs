using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.PeripheralDevices
{
    internal class Account
    {
        public Account(int accountNum, bool accountStatus, int pin, string cardHolder, int balance, int maxAllowableWithdraw)
        {
            this.accountNum = accountNum;
            this.accountStatus = accountStatus;
            this.pin = pin;
            this.cardHolder = cardHolder;
            this.balance = balance;
            this.maxAllowableWithdraw = maxAllowableWithdraw;
        }
        

        public int accountNum { get; set; }
        public bool accountStatus { get; set; }
        public int pin { get; set; }
        public string cardHolder { get; set; }
        public int balance { get; set; }

        public int maxAllowableWithdraw { get; set; }


    }
    internal class AccountDatabase
    {
        // contains CardID, CardPIN, CurrentBalance (simple!!)
        // as in a database, it's better that this is a table of three values (not 3d) with ID as the primary key
        // thus, this is a 2d array [CardID, (CardPIN, CurrentBalance)]

        private int databaseSize;
        private List<Account> sysAccount;

        public AccountDatabase()
        {
            databaseSize = 0;
            sysAccount = new List<Account>();
        }
    }
