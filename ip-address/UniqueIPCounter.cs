using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ip_address
{
    public class UniqueIPCounter
    {
        public int Compute(IEnumerable<string> ipAddresses)
        {
            int size = ipAddresses.Count();
            bool[] isChecked = new bool[size];

            int count = 0;

            for (int i = 0; i < size; i++)
            {
                if (isChecked[i])
                    continue;

                isChecked[i] = true;
                count++;
                string ip = ipAddresses.ElementAt(i);

                for (int j = i + 1; j < size; j++)
                {
                    if (ip == ipAddresses.ElementAt(j))
                        isChecked[j] = true;
                }
            }

            return count;
        }
    }
}
