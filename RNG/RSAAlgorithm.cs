using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RNG
{
    internal class RSAAlgorithm
    {
        RSAParameters RSApublicKey;
        RSAParameters RSAprivateKey;
        RSAFun RSAFunObject;
        const int minRange =108808;
        const int keyLength = 2048;
        public RSAAlgorithm(byte[] keysData)
            {
            creatingKeys(keysData);
        }
        private static RSACryptoServiceProvider RSA1 = new RSACryptoServiceProvider(keyLength);
    private void creatingKeys(byte[] keysData)
    {
            RSAprivateKey = RSA1.ExportParameters(true);
            RSApublicKey = RSA1.ExportParameters(false);
    
    int nums = 0;
    BigInteger[] pq= new BigInteger[2];
for(int i=0;i<keysData.Length-keyLength;i++)
{
var key = keysData.Skip(i).Take(keyLength / (sizeof(byte) ^ 8 ^ 2)).ToArray();
    BigInteger bI = new BigInteger(key);
if (RSAFun.IsProbablyPrime(bI))
{pq[nums++]=bI;
if(nums==2)break;}
var debug = 1;
}
BigInteger n = pq[0] * pq[1];
BigInteger phi = (pq[0] - 1) * (pq[1] - 1);
BigInteger temp1;
BigInteger e;
List<BigInteger> exponentList = new List<BigInteger>();
Random random = new Random();
for (BigInteger i = 380; i < 1000000; i++)
{
                var gcd = RSAFun.euclid(phi,i);
    if (gcd.IsOne) exponentList.Add(i);
}

BigInteger temp2;
(BigInteger, BigInteger) secret;
do
{
    e = exponentList[random.Next() % exponentList.Count];
    secret = RSAFun.exteuclid(phi, e);
} while (!secret.Item1.IsOne);
  temp2 = secret.Item2;

var inverseQ = RSAFun.exteuclid(pq[0], pq[1]).Item2;

RSAParameters publicKey = new RSAParameters();
publicKey.Modulus = RSAFun.BigIntegerToRSAByteArray(n);
publicKey.Exponent = RSAFun.BigIntegerToRSAByteArray(e);
RSAParameters privateKey = new RSAParameters();
privateKey.Modulus = RSAFun.BigIntegerToRSAByteArray(n);//modulus
privateKey.D = RSAFun.BigIntegerToRSAByteArray(n); //privateExponent
privateKey.Exponent = RSAFun.BigIntegerToRSAByteArray(e);//publicExponent
privateKey.P = RSAFun.BigIntegerToRSAByteArray(pq[0]);//prime1
privateKey.Q= RSAFun.BigIntegerToRSAByteArray(pq[1]);//prime2
privateKey.DP= RSAFun.BigIntegerToRSAByteArray(temp2 & (pq[0]-1));//d mod (p-1) wykładnik1
privateKey.DQ= RSAFun.BigIntegerToRSAByteArray(temp2 & (pq[1] - 1));// d mod (q - 1)	wykładnik2
privateKey.InverseQ = RSAFun.BigIntegerToRSAByteArray(inverseQ);//Współczynnik
RSApublicKey = publicKey;
RSAprivateKey= privateKey;
            var debug2 = 2;
        }



        public bool isSignatureVeryfied(string data) 
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[]? signed;
            signed = RSAHashAndSign(bytes, RSAprivateKey);
            return verifySign(bytes,signed,RSApublicKey);
        }

        private byte[]? RSAHashAndSign(byte[] bytes, RSAParameters RSAprivateKey)
        {
            try
            {
                RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
                RSA2.ImportParameters(RSAprivateKey);
                return RSA2.SignData(bytes, SHA256.Create());
            }
            catch (CryptographicException e) 
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private bool verifySign(byte[] bytes, byte[] SignedBytes, RSAParameters RSAKey) 
        {
            RSACryptoServiceProvider RSA2=new RSACryptoServiceProvider();
            RSA2.ImportParameters(RSAKey);
            //here is null
            return RSA2.VerifyData(bytes, SHA256.Create(),SignedBytes);
        }
        public string getStrings(byte[] temp)
        {
            return temp.ToString();

        }
     

        public byte[] Encrypt(string text) 
        {
            RSA1.ImportParameters(RSApublicKey);
            var bytesPlainTextData = Encoding.Unicode.GetBytes(text);
            byte[] encryptedData = RSA1.Encrypt(bytesPlainTextData, true);
            return encryptedData;
        }
       public byte[] Decrypt(byte[] temp)
        {
            RSA1.ImportParameters(RSAprivateKey);
            var bytesPlainTextData = RSA1.Decrypt(temp,false);
            byte[] encryptedData = RSA1.Encrypt(bytesPlainTextData, true);
            return encryptedData;
        }
        public RSAParameters PublicKey() 
        {
            return RSApublicKey;
        }
        public RSAParameters PrivateKey()
        {
            return RSAprivateKey;
        }
    }
}
