using System;
using System.IO;
using System.Security.Cryptography;

namespace HyperLogLog
{
    class Program
    {
        static void Main(string[] args)
        {
            const int ACCURANCE = 16;

            var hashAlgorithm = HashAlgorithm.Create(HashAlgorithmName.SHA256.Name);
            //string fileName = "D:\\Temp\\data.txt";
            string fileName = "data.txt";

            var result = Count(fileName, hashAlgorithm, ACCURANCE);

            Console.WriteLine(result);
        }

        static double Count(string fileName, HashAlgorithm hashAlgorithm, int accuracy)
        {
            var hll = new HyperLogLog(hashAlgorithm, accuracy);

            using (StreamReader streamReader = new StreamReader(File.OpenRead(fileName)))
            {
                while (!streamReader.EndOfStream)
                {
                    hll.LogData(streamReader.ReadLine());
                }
            }

            var cardinality = hll.GetCount();

            return cardinality;
        }
    }
}
