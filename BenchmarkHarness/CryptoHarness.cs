using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;

namespace BenchmarkHarness
{
    class CryptoHarness
    {
        

        private readonly StringBuilder sb = new StringBuilder();
        private readonly int _inputSize;
        private readonly byte[] _message;
        private readonly Random _prng = new Random(18239);
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly int runs = 1000;

        private byte[] _cryptoText;
        private byte[] _aux;

        public CryptoHarness(int inputSize) : this(inputSize,100000)
        {
            
        }

        public CryptoHarness(int inputSize, int runs)
            
        {
            this._inputSize = inputSize;
            //fill the message with some random bytes
            this._message = new byte[this._inputSize];
            this._prng.NextBytes(_message);
            this._cryptoText = new byte[this._inputSize];
            this._aux = new byte[this._inputSize];
            this.sb.Append("==============================================================================================================" + Environment.NewLine +
                            "[" + DateTime.Now.ToString() + "] Starting crypto benchmarking..." + Environment.NewLine +
                            "Message to encrypt: " + BitConverter.ToString(this._message) + Environment.NewLine +
                            "Input size: " + this._inputSize + Environment.NewLine +
                            "Number of runs: " + runs + Environment.NewLine + Environment.NewLine);
            this.runs = runs;

        }
        

        public void RunHarness(StreamWriter sw)
        {
            this.MakeHash(MD5CryptoServiceProvider.Create, "MD5");
            this.MakeHash(SHA256CryptoServiceProvider.Create, "SHA256");
            this.MakeHash(SHA384CryptoServiceProvider.Create, "SHA384");
            this.MakeHash(SHA512CryptoServiceProvider.Create, "SHA512");
            if (this._inputSize > 0)
            {
                this.MakeEncription(AesCryptoServiceProvider.Create, "AES");            
                
            }
            
            this.MakeDSA();
            
            

            sw.Write(sb.ToString());           
        }

        public void RunRsaHarness(StreamWriter sw)
        {
            MakeRSA(1024);
            MakeRSA(2048);
            MakeRSA(4096);
            sw.Write(sb);
        }

        private void MakeRSA(int len)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(len);
            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                this._cryptoText = rsa.Encrypt(_message,false);
            }
            this._stopwatch.Stop();
            this.prettyPrint("RSA"+len+" Encrypt");

            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                rsa.Decrypt(this._cryptoText, false);
            }
            this._stopwatch.Stop();
            this.prettyPrint("RSA"+len+" Decrypt");
        }
        private void MakeDSA()
        {
            var hash = SHA1.Create().ComputeHash(this._message);
            DSA dsa = DSACryptoServiceProvider.Create();
            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                _aux = dsa.CreateSignature(hash);
            }
            this._stopwatch.Stop();
            prettyPrint("DSA sign");

            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                dsa.VerifySignature(hash, _aux);
            }
            this._stopwatch.Stop();
            prettyPrint("DSA verify");
            
            
        }

        private delegate HashAlgorithm HashAlgorithmCreatorDelegate();
        private void MakeHash(HashAlgorithmCreatorDelegate creator, string algo)
        {
            HashAlgorithm cryptoAlg = creator();
            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                cryptoAlg.ComputeHash(this._message);
            }
            this._stopwatch.Stop();
            prettyPrint(algo);
            
        }

        private delegate SymmetricAlgorithm SymmetricAlgorithmDelegate();
        private void MakeEncription(SymmetricAlgorithmDelegate creator, string algo)
        {
           
            SymmetricAlgorithm cryptoAlg = creator();

            ICryptoTransform enc = cryptoAlg.CreateEncryptor(cryptoAlg.Key, cryptoAlg.IV);           
            ICryptoTransform decr = cryptoAlg.CreateDecryptor(cryptoAlg.Key, cryptoAlg.IV);
           
            
            this._stopwatch.Reset();
            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                enc.TransformBlock(_message, 0, this._inputSize, this._cryptoText, 0);
                
            }

            this._stopwatch.Stop();       
            this.prettyPrint(algo + "Encrypt");

            this._stopwatch.Start();
            for (int i = 0; i < runs; i++)
            {
                decr.TransformBlock(_message, 0, this._inputSize, this._cryptoText, 0);

            }

            this._stopwatch.Stop();
            this.prettyPrint(algo + "Decrypt");

        }

        public void prettyPrint(string algo)
        {
            sb.Append("Runned " + algo + " " + "Time/run: " + this._stopwatch.ElapsedMilliseconds / Convert.ToDouble(runs) + " [ms] " +
                "Ticks: " + this._stopwatch.ElapsedTicks/Convert.ToDouble(runs)   + " Stopwatch frequency: " + Stopwatch.Frequency + " Ticks/sec" + Environment.NewLine);
            this._stopwatch.Reset();
        }


        //public delegate HashAlgorithm HashAlgorithmCreatorDelegate();
        //public void MakeHash<T>(HashAlgorithmCreatorDelegate creator) where T : HashAlgorithm
        //{
        //    T cryptoAlg = creator();
        //}
       
    }
}
