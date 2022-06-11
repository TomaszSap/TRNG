using RNG;
using NAudio;
using NAudio.Wave;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;



public static class MainProgram
{
    public static string[] table = new string[7];

    public static void startApp(string text)
    {
        PostProcessingRNG postProcessingRNG = new PostProcessingRNG();
        RSAAlgorithm rsa = new RSAAlgorithm(postProcessingRNG.GetRandomData());
        var coding = rsa.Encrypt(text);
        var codedSign = coding.ToString;
        table[0] = "RSA public key: "+rsa.PublicKey();
        Console.WriteLine($"RSA public key: {rsa.PublicKey()}");
        table[1] = "RSA private key: "+rsa.PrivateKey();
        Console.WriteLine($"RSA private key: {rsa.PrivateKey()}");


        table[2] = "Text:"+text;
        Console.WriteLine($"Text:{text}");
        table[3] = "Coded text: "+ codedSign;
        Console.WriteLine($"Coded text:{coding}");


        var isveryfied = rsa.isSignatureVeryfied(text);
        var p = Encoding.UTF8.GetBytes(text);

        var x = new Org.BouncyCastle.Crypto.Digests.Sha3Digest(512);
        x.BlockUpdate(p, 0, p.Length);
        var sha3hash = new byte[512 / 8];
        x.DoFinal(sha3hash, 0);

        table[4] = $"Starting signature:";
        Console.WriteLine($"Starting signature:");
        table[5] = "hashed signature sha256:" + Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(text)));
        Console.WriteLine("hashed signature sha256:" + Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(text))));
        table[6] = (isveryfied) ? "SUCCESSFUL ENCRYPTED TEXT:" + text : "SIGNATURE INVALID";
        Console.WriteLine((isveryfied) ? "SUCCESSFUL ENCRYPTED TEXT:" + text : "SIGNATURE INVALID");


    }
    public static void Main() 
    {
    }
}