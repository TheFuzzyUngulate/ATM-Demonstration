using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GUIEnabledATM.InternalDevices
{
    internal class CashDisburser
    {
        internal (uint, uint, uint, uint, uint) _bills;

        public CashDisburser()
        {
            _bills = (0, 0, 0, 0, 0);
        }

        public string Disburse()
        {
            string res = "";
            List<uint> bills_int = new() { _bills.Item1, _bills.Item2, _bills.Item3, _bills.Item4, _bills.Item5 };
            List<string> strings = new();

            for (int i = 0; i < bills_int.Count; i++)
            {
                if (bills_int[i] > 0)
                {
                    string ind = "";
                    ind += bills_int[i].ToString();
                    ind += " $";
                    ind += Convert.ToInt32(Math.Pow(10, i / 3) * (5 * Math.Pow(2, i % 3))).ToString();
                    ind += ((bills_int[i] > 1) ? " bills" : " bill");
                    strings.Add(ind);
                }
            }

            while (strings.Count > 0)
            {
                string cur = strings[0];
                strings.RemoveAt(0);

                res += cur;
                if (strings.Count > 1) res += ", ";
                else if (strings.Count == 1) res += " and ";
                else if (strings.Count == 0) res += " disbursed.\n";
            }

            if (res == "")
            {
                res += "No bills were disbursed.\n";
            }

            return res;
        }
    }
}
