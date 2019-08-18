using System;
using System.IO;
using System.Security.Cryptography;

namespace DeCr
{
    class DecryptYY
    {
        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
        public static string BytetoSymb(byte[][] bt)
        {
            string ss = "";
            for (int i = 0; i < bt.Length; i++)
            {
                for (int j = 0; j < bt[i].Length; j++)
                {
                    ss += (char)bt[i][j];
                }
            }
            return ss;
        }
        public static byte[] BytetoOneArray(byte[][] bt)
        {
            byte[] res = new byte[16 * bt.Length];
            for (int i = 0; i < bt.Length; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    res[i * 16 + j] = bt[i][j];
                }
            }
            return res;
        }

     
    }
}
