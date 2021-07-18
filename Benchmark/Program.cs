using BenchmarkDotNet.Attributes;
using System;
using ip_address;
using System.Collections;
using System.Collections.Generic;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    public class Md5VsSha256
    {
        private const int N = 100_000_000;
        private readonly IEnumerable<string> data;

        ip_address.Program.UniqueIPCounter treeCounter;
        ip_address.UniqueIPCounter boolCounter;


        public Md5VsSha256()
        {
            data = new ip_address.Program.IPAddressGenerator().GenerateIP(N);
            //data = MyGen();
            treeCounter = new();
            boolCounter = new();
        }

        [Benchmark]
        public long Tree() => treeCounter.Compute(data);

        [Benchmark]
        public long Alex() => GetUniqueIpCount(data);

        private long GetUniqueIpCount(IEnumerable<string> ips)
        {
            if (ips == null)
                return 0;
            var uniqueIps = new HashSet<string>();
            foreach (var ip in ips)
                uniqueIps.Add(ip);
            return uniqueIps.Count;
        }

        private IEnumerable<string> MyGen()
        {
            int size = 0;
            string[] ips = new string[140* 140* 140* 140];

            for (int i = 0; i < 140; i++)
            {
                for (int j = 0; j < 140; j++)
                {
                    for (int k = 0; k < 140; k++)
                    {
                        for (int l = 0; l < 140; l++)
                        {
                            ips[size] = $"{i}.{j}.{k}.{l}";
                        }
                    }
                }
            }

            return ips;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
