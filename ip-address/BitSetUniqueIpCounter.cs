using System.IO;
using System.Collections;

namespace ip_address
{
    public class BitSetUniqueIpCounter
    {
        private readonly BitArray _firstBitArray;
        private readonly BitArray _secondBitArray;
        private int _count;

        public BitSetUniqueIpCounter()
        {
            _firstBitArray = new BitArray(int.MaxValue, false);
            _secondBitArray = new BitArray(int.MaxValue, false);
            _count = 0;
        }

        public int Compute(string fileName)
        {
            var workBitArray = _firstBitArray;
            int indexInArray;
            string line;

            using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
            {
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    long ipNumber = GetIPNumber(line);

                    if (ipNumber > int.MaxValue)
                    {
                        ipNumber -= int.MaxValue;
                        workBitArray = _secondBitArray;
                    }

                    indexInArray = (int)ipNumber;

                    if (!workBitArray.Get(indexInArray))
                    {
                        _count++;
                        workBitArray.Set(indexInArray, true);
                    }
                }
            }

            return _count - 1;
        }

        private long GetIPNumber(string ip)
        {
            long ipNumber = 0;
            var octets = ip.Split('.');

            for (int i = 0; i < 4; i++)
            {
                ipNumber += int.Parse(octets[i]) * LongPow(256, 3 - i);
            }

            return ipNumber;
        }

        private static long LongPow(int x, int pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
    }
}
