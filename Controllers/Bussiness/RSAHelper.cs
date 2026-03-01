using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Gemini.Controllers.Bussiness
{
    public class RSAHelper
    {
        public void GenerateKeys(string keyContainer, out string privateKeyXML, out string publicKeyXML)
        {
            CspParameters cspParams;
            cspParams = new CspParameters(1);
            cspParams.KeyContainerName = string.Format("{0}{1}", keyContainer, GeminiController.Key);
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            var rsa = new RSACryptoServiceProvider(cspParams);

            //Pair of public and private key as XML string.
            //Do not share this to other party
            privateKeyXML = rsa.ToXmlString(true);

            //Private key in xml file, this string should be share to other parties
            publicKeyXML = rsa.ToXmlString(false);
        }

        public string ConvertStringToHash(string text)
        {
            SHA256 mySHA256 = SHA256.Create();

            string hash = string.Empty;
            using (var stream = GenerateStreamFromString(text))
            {
                byte[] by = mySHA256.ComputeHash(stream);

                hash = TranformBinary(by);
            }

            return hash;
        }

        public string SignData(string message, string privateKeyXML)
        {
            //// The array to store the signed message in bytes
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                //// Write the message to a byte array using UTF8 as the encoding.
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(message);

                try
                {
                    //// Import the private key used for signing the message
                    rsa.FromXmlString(privateKeyXML);

                    //// Sign the data, using SHA256 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA256"));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            //// Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }

        public bool VerifyData(string originalMessage, string signedMessage, string publicKeyXML)
        {
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] bytesToVerify = encoder.GetBytes(originalMessage);
                byte[] signedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    rsa.FromXmlString(publicKeyXML);

                    SHA256Managed Hash = new SHA256Managed();

                    byte[] hashedData = Hash.ComputeHash(signedBytes);

                    success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private string TranformBinary(byte[] data)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                return string.Concat(sha1.ComputeHash(data).Select(x => x.ToString("X2")));
            }
        }
    }
}