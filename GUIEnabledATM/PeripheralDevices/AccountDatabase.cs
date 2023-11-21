using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.PeripheralDevices
{
    internal class AccountDatabase
    {
        // contains CardID, CardPIN, CurrentBalance (simple!!)
        // as in a database, it's better that this is a table of three values (not 3d) with ID as the primary key
        // thus, this is a 2d array [CardID, (CardPIN, CurrentBalance)]

        private int databaseSize;
        private Dictionary<int, (int PIN, int balance)> dataHolder;

        public AccountDatabase()
        {
            databaseSize = 0;
            dataHolder = new();
            AddAccount(1, 4020, 1000);
            AddAccount(2, 1111, 55);
            AddAccount(3, 5712, 1000);
        }

        public int GetSize() {  return databaseSize; }

        public void AddAccount(int card_id, int card_pin)
        {
            dataHolder.Add(card_id, (card_pin, 0));
            databaseSize++;
        }

        public void AddAccount(int card_id, int card_pin, int balance)
        {
            AddAccount(card_id, card_pin);
            SetBalance(card_id, balance);
        }

        public int GetBalance(int card_id)
        {
            if (!dataHolder.ContainsKey(card_id))
            {
                throw new ArgumentOutOfRangeException("Key passed in card_id argument does not exist");
            }
            return dataHolder[card_id].balance;
        }

        public void SetBalance(int card_id, int balance)
        {
            if (!dataHolder.ContainsKey(card_id))
            {
                throw new ArgumentOutOfRangeException("Key passed in card_id argument does not exist");
            }
            dataHolder[card_id] = (dataHolder[card_id].PIN, balance);
        }

        public void Withdraw(int card_id, int offset)
        {
            if (!dataHolder.ContainsKey(card_id))
            {
                throw new ArgumentOutOfRangeException("Key passed in card_id argument does not exist");
            }
            dataHolder[card_id] = (dataHolder[card_id].PIN, dataHolder[card_id].balance - offset);
        }

        public void Login(int card_id, int card_pin)
        {
            if (!dataHolder.ContainsKey(card_id))
            {
                throw new ArgumentOutOfRangeException("Key passed in card_id argument does not exist");
            }
            var (PIN, _) = dataHolder[card_id];
            if (card_pin != PIN)
            {
                throw new ArgumentException("Given card_pin does not match the stored pin");
            }
        }

        public void DeleteAccount(int card_id)
        {
            try
            {
                dataHolder.Remove(card_id);
                databaseSize--;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
