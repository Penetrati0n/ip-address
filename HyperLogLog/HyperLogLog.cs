using System;
using System.Linq;
using System.Security.Cryptography;

namespace HyperLogLog
{
    public class HyperLogLog
    {
        internal readonly byte[] _registers;
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly IBytesConverter _bytesConverter;
        
        public const double DOUBLE_MAX_VALUE = double.MaxValue;

        public int B { get; private set; }
        public int M { get; private set; }
        public double Alpha { get; private set; }



        public HyperLogLog(int b = 10)
            : this(MD5.Create(), new BytesConverter(), b)
        { }

        public HyperLogLog(HashAlgorithm hashAlgorithm, int b = 10)
            : this(hashAlgorithm, new BytesConverter(), b)
        { }

        public HyperLogLog(IBytesConverter bytesConverter, int b = 10)
            : this(MD5.Create(), bytesConverter, b)
        { }

        public HyperLogLog(HashAlgorithm hashAlgorithm, IBytesConverter bytesConverter, int b = 10)
        {
            if (b < 4 || b > 16)
                throw new ArgumentOutOfRangeException("b", string.Format("HyperLogLog accuracy of {0} is not supported. Please use a B value between 4 and 16.", b));

            _hashAlgorithm = hashAlgorithm;
            _bytesConverter = bytesConverter;
            B = b;
            M = IntPow(2, b);

            Alpha = ComputeAlpha();
            _registers = new byte[M];
        }


        public void LogData(object data)
        {
            var hash = GetHash(data);

            LogHash(hash);
        }

        public void LogHash(ulong hash)
        {
            var registerIndex = GetRegisterIndex(hash); // Двоичный адрес крайних правых битов.
            var runLength = RunOfZeros(hash);           // Длина последовательности нулей, начинающейся с бита b+1.
            _registers[registerIndex] = Math.Max(_registers[registerIndex], runLength);
        }



        public double GetCount()
        {
            double dv;

            var registersSum = _registers.Sum(r => 1d / FastPow2(r));
            var dvEstimate = Alpha * ((double)M * M) / registersSum;

            if (dvEstimate < 2.5 * M)
            {
                var v = _registers.Count(r => r == 0);
                if (v == 0)
                {
                    dv = dvEstimate;
                }
                else
                {
                    dv = M * Math.Log((double)M / v);
                }
            }
            else if (dvEstimate <= (DOUBLE_MAX_VALUE * 1 / 30))
            {
                dv = dvEstimate;
            }
            else
            {
                if (dvEstimate <= double.MaxValue)
                {
                    dv = -DOUBLE_MAX_VALUE * Math.Log(1 - dvEstimate / DOUBLE_MAX_VALUE);
                }
                else
                {
                    throw new ArgumentException($"Estimated cardinality exceeds {double.MaxValue}");
                }
            }

            return Math.Round(dv, 0);
        }

        private static double FastPow2(int x)
        {
            return (ulong)1 << x;  // right up to 2^63
        }

        private ulong GetHash(object data)
        {
            var dataBytes = _bytesConverter.GetBytes(data);
            var hashBytes = _hashAlgorithm.ComputeHash(dataBytes);

            return BitConverter.ToUInt64(hashBytes, 0);
        }

        private uint GetRegisterIndex(ulong hash)
        {
            var index = hash & ((uint)Math.Pow(2, B) - 1);
            return (uint)index;
        }

        private byte RunOfZeros(ulong hash)
        {
            var value = hash >> B;

            var shifted = 0;
            byte count = 1;
            while (((value & 1) == 0) && shifted < 64)
            {
                value >>= 1;
                count++;
                shifted++;
            }
            return count;
        }

        private static int IntPow(int x, int pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }


        private double ComputeAlpha()
        {
            return M switch
            {
                16 => 0.673,
                32 => 0.697,
                64 => 0.709,
                _ => 0.7213 / (1 + (1.079 / M)),
            };
        }


        public static HyperLogLog Merge(params HyperLogLog[] hlls)
        {
            if (hlls == null || hlls.Length == 0)
                return new HyperLogLog();

            var b = hlls[0].B;

            if (hlls.Any(hyperLogLog => b != hyperLogLog.B))
                throw new ArgumentException("All HyperLogLogs needs to be on the same size", "hlls");


            var result = new HyperLogLog(hlls[0]._hashAlgorithm, hlls[0]._bytesConverter, hlls[0].B);

            for (int i = 0; i < result.M; i++)
            {
                result._registers[i] = hlls.Max(x => x._registers[i]);
            }

            return result;
        }
    }
}
