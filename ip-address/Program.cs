using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

namespace ip_address
{
    public class Program
    {
        static void Main(string[] args)
        {
            //BitArray bitArray = new BitArray(2);
            //Console.WriteLine(bitArray.Get(0));
            //bitArray.Set(0, true);
            //Console.WriteLine(bitArray.Get(0));

            const int COUNT_IP = 100_000_000;
            string fileName = "D:\\Temp\\data.txt";

            //var worker = new UniqueIPCounter();
            var worker2 = new BitSetUniqueIpCounter();
            var generator = new IPAddressGenerator();
            //generator.GenerateIP(COUNT_IP, fileName);
            //var ips = generator.GenerateIP(COUNT_IP);
            //    var ips = new IPAddressReader().ReadIPFromFile("data.txt");

            //    long result = worker.Compute(ips);
            long result2 = worker2.Compute(fileName);

            //    Console.WriteLine(result);
            Console.WriteLine(result2);
        }


        ////////////// Считающий класс //////////////
        public class UniqueIPCounter
        {
            private readonly Dictionary<byte, Dictionary<byte, Dictionary<byte, HashSet<byte>>>> _ipTree;
            private long _count;

            public UniqueIPCounter()
            {
                _ipTree = new();
                _count = 0;
            }

            public long Compute(IEnumerable<string> ipAddresses)
            {
                foreach (var ip in ipAddresses)
                {
                    if (string.IsNullOrEmpty(ip))
                        continue;
                    var bytesOfIP = ParseIpAddress(ip);
                    Check(bytesOfIP);
                }

                return _count;
            }

            private void Check(byte[] ipAddress)
            {
                if (_ipTree.ContainsKey(ipAddress[0]))
                {
                    if (_ipTree[ipAddress[0]].ContainsKey(ipAddress[1]))
                    {
                        if (_ipTree[ipAddress[0]][ipAddress[1]].ContainsKey(ipAddress[2]))
                        {
                            if (!_ipTree[ipAddress[0]][ipAddress[1]][ipAddress[2]].Contains(ipAddress[3]))
                            {
                                _ipTree[ipAddress[0]][ipAddress[1]][ipAddress[2]].Add(ipAddress[3]);

                                _count++;
                            }
                        }
                        else
                        {
                            _ipTree[ipAddress[0]][ipAddress[1]].Add(ipAddress[2], new());
                            _ipTree[ipAddress[0]][ipAddress[1]][ipAddress[2]].Add(ipAddress[3]);

                            _count++;
                        }
                    }
                    else
                    {
                        _ipTree[ipAddress[0]].Add(ipAddress[1], new());
                        _ipTree[ipAddress[0]][ipAddress[1]].Add(ipAddress[2], new());
                        _ipTree[ipAddress[0]][ipAddress[1]][ipAddress[2]].Add(ipAddress[3]);

                        _count++;
                    }
                }
                else
                {
                    _ipTree.Add(ipAddress[0], new());
                    _ipTree[ipAddress[0]].Add(ipAddress[1], new());
                    _ipTree[ipAddress[0]][ipAddress[1]].Add(ipAddress[2], new());
                    _ipTree[ipAddress[0]][ipAddress[1]][ipAddress[2]].Add(ipAddress[3]);

                    _count++;
                }
            }

            private static byte[] ParseIpAddress(string ipAddress)
            {
                const int IP_SIZE = 4;

                byte[] parsed_ip = new byte[IP_SIZE];
                string[] ip_bytes = ipAddress.Split('.');

                for (int i = 0; i < IP_SIZE; i++)
                {
                    parsed_ip[i] = byte.Parse(ip_bytes[i]);
                }

                return parsed_ip;
            }
        }
        /////////////////////////////////////////////
        
        public class IPAddressReader
        {
            private readonly Regex _ipRegex;
            private readonly char[] _separators;

            public IPAddressReader()
            {
                _ipRegex = new Regex("[0-9]+.[0-9]+.[0-9]+.[0-9]+");
                _separators = new char[] {' ', ',', '\n', '\r'};
            }

            public IEnumerable<string> ReadIPFromFile(string fileName) => File.ReadAllText(fileName).Split(_separators).Where(ip => IsValidIP(ip));

            private bool IsValidIP(string ip) => _ipRegex.IsMatch(ip);
        }

        public class IPAddressGenerator
        {
            private readonly Random _random;
            private readonly byte[] _buffer;
            private const int IP_SIZE = 4;

            public IPAddressGenerator()
            {
                _random = new Random();
                _buffer = new byte[IP_SIZE];
            }

            public void GenerateIP(int count, string fileName)
            {
                int lastPrint = 0;
                for (int i = 0; i < count; i++)
                {
                    _random.NextBytes(_buffer);
                    File.AppendAllText(fileName, string.Join('.', _buffer) + "\n");

                    if (lastPrint != i / 100_000)
                    {
                        lastPrint = i / 100_000;
                        Console.WriteLine($"{i, 10}/{count}");
                    }
                }
            }
        }
    }
}
