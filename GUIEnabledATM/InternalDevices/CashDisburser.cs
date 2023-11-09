using GUIEnabledATM.Utilities;
using System;
using System.Collections.Generic;
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

        internal void Disburse()
        {
            string str = "";
            List<uint> bills_int = new() { _bills.Item1, _bills.Item2, _bills.Item3, _bills.Item4, _bills.Item5 };

            while (bills_int.Count > 0)
            {
                uint x = bills_int[0];

                if (x != 0)
                {
                    str += x;
                    int denom = 5 - bills_int.Count;
                    switch (denom)
                    {
                        case 0:
                            str += " 5";
                            break;
                        case 1:
                            str += " 10";
                            break;
                        case 2:
                            str += " 20";
                            break;
                        case 3:
                            str += " 50";
                            break;
                        case 4:
                            str += " 100";
                            break;
                    }
                    str += "$ bill" + ((x == 1) ? "" : "s");
                    if (bills_int.Count == 1)
                    {
                        str += " omitted.";
                    }
                    else
                    {
                        str += ",";
                        if (bills_int.Count == 2)
                        {
                            str += " and";
                        }
                    }
                    str += "\n";
                }

                bills_int.RemoveAt(0);
            }

            if (str == "")
            {
                str += "No bills were disbursed.\n";
            }

            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
