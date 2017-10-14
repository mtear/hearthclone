using System;
using System.Security.Cryptography;

namespace HS_Net
{

    /// <summary>
    /// A class to handle RSA encryption in a very easy to use way
    /// </summary>
    public class RSAHandler
    {
        /// <summary>
        /// Create an RSA public/private key pair set of xml strings
        /// </summary>
        /// <param name="size">The byte size of the key. It might be best to use 1024 or 2048</param>
        /// <returns>A string array of keys. [0] = Public key, [1] = Private key</returns>
        public static string[] GenerateKeyPair(int size)
        {
            //Create return object
            string[] keys = new string[2];

            //Create the CSP
            var csp = new RSACryptoServiceProvider(size);

            //Assign values
            keys[0] = CreateRSAString(csp.ExportParameters(false));
            keys[1] = CreateRSAString(csp.ExportParameters(true));

            //Return
            return keys;
        }

        /// <summary>
        /// Create an xml string from an RSA Parameters object
        /// </summary>
        /// <param name="p">An RSA Parameters object</param>
        /// <returns>An xml string of the RSA Paramters</returns>
        private static string CreateRSAString(RSAParameters p)
        {
            var stringWriter = new System.IO.StringWriter();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xmlSerializer.Serialize(stringWriter, p);
            //Get the string from the Stream
            return stringWriter.ToString();
        }

        /// <summary>
        /// Create an RSAParameters object from an RSA xml string
        /// </summary>
        /// <param name="key">An RSA xml string</param>
        /// <returns>An RSA Paramters object representing the given string or null if error</returns>
        private static RSAParameters CreateRSAParametersFromString(string key)
        {
            var stringReader = new System.IO.StringReader(key);
            var xmlDeserializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //Get the object from the deserializer
            return (RSAParameters)xmlDeserializer.Deserialize(stringReader);
        }

        /// <summary>
        /// Encrypt some string data using the given RSA public key
        /// </summary>
        /// <param name="publicKey">The public key to encrypt with</param>
        /// <param name="data">The data to encrypt</param>
        /// <returns>A Base 64 encoded string of the encrypted data or null if an error occurred</returns>
        public static string Encrypt(string publicKey, string data)
        {
           // try
           // {
                //Create the RSA Parameters from the key
                RSAParameters pubKey = CreateRSAParametersFromString(publicKey);

                //Create the CSP
                var csp = new RSACryptoServiceProvider();
                csp.ImportParameters(pubKey);

                //Encrypt and convert to Base64
                var bytesPlainTextData = System.Text.Encoding.UTF8.GetBytes(data);
                var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                return Convert.ToBase64String(bytesCypherText);
           // }
           // catch
           // {
           //     return null;
           // }
        }

        /// <summary>
        /// Decrypt some cypher text with the given RSA private key
        /// </summary>
        /// <param name="privateKey">The private key to decrypt with</param>
        /// <param name="cypherText">The cypher text to decrypt as a Base64 string</param>
        /// <returns>Decrypted plaintext of the cypher, or null if there was an error</returns>
        public static string Decrypt(string privateKey, string cypherText)
        {
          //  try
           // {
                //Create the RSA Parameters from the key
                RSAParameters privKey = CreateRSAParametersFromString(privateKey);

                //Create the CSP
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.ImportParameters(privKey);

                //Convert from Base64 and Decrypt
                byte[] bytesCypherText = Convert.FromBase64String(cypherText);
                byte[] bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                return System.Text.Encoding.UTF8.GetString(bytesPlainTextData);
          //  }
          //  catch
          //  {
          //      return null;
          //  }
        }

    }
}
