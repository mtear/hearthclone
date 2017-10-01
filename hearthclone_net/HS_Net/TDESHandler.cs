using System;
using System.IO;
using System.Security.Cryptography;

namespace HS_Net
{

    public class TDESHandler
    {

        public static string GenerateKey()
        {
            TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();
            TDES.GenerateKey();
            return System.Text.Encoding.ASCII.GetString(TDES.Key);
        }

        /* public static string Encrypt(string key, string data)
         {
             TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();
             TDES.GenerateIV();
             TDES.Key = System.Text.Encoding.ASCII.GetBytes(key);

             byte[] bdata = System.Text.Encoding.ASCII.GetBytes(data);

             MemoryStream ms = new MemoryStream();
             CryptoStream cStream = new CryptoStream(ms,
                 TDES.CreateEncryptor(),
                 CryptoStreamMode.Write);

             StringWriter sr = new StringWriter(cStream);

             string cypher = Convert.ToBase64String(System.Text.Encoding.ASCII.GetString(ms.ToArray()));

             cStream.Close();
             ms.Close();

             return cypher;

         }

         public static string Decrypt(string key, string cypher)
         {
             TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();
             TDES.GenerateIV();
             TDES.Key = System.Text.Encoding.ASCII.GetBytes(key);

             byte[] bdata = Convert.FromBase64String(cypher);

             MemoryStream ms = new MemoryStream();
             CryptoStream cStream = new CryptoStream(ms,
                 TDES.CreateDecryptor(),
                 CryptoStreamMode.Write);

             // Encrypt the input plaintext string
             cStream.Write(bdata, 0, bdata.Length);

             // Complete the encryption process
             cStream.FlushFinalBlock();

             string text = System.Text.Encoding.ASCII.GetString(ms.ToArray());
             return text;
         }*/

        public static byte[] Encrypt(string key, string data)
        {
            TripleDES des = CreateDES(key);
            ICryptoTransform ct = des.CreateEncryptor();
            byte[] input = System.Text.Encoding.Unicode.GetBytes(data);
            return ct.TransformFinalBlock(input, 0, input.Length);
        }

        public static string Decrypt(string key, string cypher)
        {
            byte[] b = Convert.FromBase64String(cypher);
            TripleDES des = CreateDES(key);
            ICryptoTransform ct = des.CreateDecryptor();
            byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
            return System.Text.Encoding.Unicode.GetString(output);
        }

        static TripleDES CreateDES(string key)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            return des;
        }

    }

}
