using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace SLOCK
{

    class BlockCrypt
    {
      public  static byte[] CorrectKey(byte[] key)
        {
            byte[] res = new byte[32];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res[j + i * 4] = key[j];
                }
            }
            return res;
        }
       public  static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

      

        public static byte[][] CryptC(BigInteger koef1, BigInteger koef2, BigInteger koef3, byte[][] EnM, byte[][] EnN)
        {
            byte[][] res = new byte[EnM.Length][];
            for (int i = 0; i < EnM.Length; i++)
            {
                res[i] = new byte[34];
                BigInteger CM = new BigInteger(EnM[i]);
                BigInteger CN = new BigInteger(EnN[i]);
                BigInteger C = BigInteger.Remainder(BigInteger.Add(BigInteger.Multiply(CM, koef1), BigInteger.Multiply(CN, koef2)), koef3);
                byte[] m = C.ToByteArray();
                for (int j = 0; j < m.Length; j++)
                {
                    res[i][j] = m[j];
                }
                res[i][33] = (byte)m.Length;
            }
            return res;

        }

        public static string BytetoSymb(byte[][] bt)
        {
            string ss="";
            for (int i = 0; i < bt.Length; i++)
            {
                for (int j = 0; j < bt[i].Length; j++)
                {
                    ss += (char)bt[i][j];
                }
            }
            return ss;
        }
    }
}
