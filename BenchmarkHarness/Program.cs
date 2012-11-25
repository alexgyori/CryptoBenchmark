using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BenchmarkHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "result" + ".txt";            
            StreamWriter sw = new StreamWriter(filename);

            CryptoHarness crsa1024 = new CryptoHarness(96,1000);
            crsa1024.RunRsaHarness(sw);

            CryptoHarness c0 = new CryptoHarness(0);
            c0.RunHarness(sw);
            CryptoHarness c128 = new CryptoHarness(128);
            c128.RunHarness(sw);
            CryptoHarness c512 = new CryptoHarness(512);
            c512.RunHarness(sw);
            CryptoHarness c1024 = new CryptoHarness(1024);
            c1024.RunHarness(sw);
            CryptoHarness c2048 = new CryptoHarness(2048);
            c2048.RunHarness(sw);
            CryptoHarness c4096 = new CryptoHarness(4096);
            c4096.RunHarness(sw);

            sw.Close();


        }
    }
}
