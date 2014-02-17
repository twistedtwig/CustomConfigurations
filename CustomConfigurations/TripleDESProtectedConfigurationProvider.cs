using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CustomConfigurations
{
    public class TripleDESProtectedConfigurationProvider : ProtectedConfigurationProvider
    {

        private TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

        private string pKeyFilePath = "configkey.txt";
        private string pName;

        // Gets the path of the file 
        // containing the key used to 
        // encryption or decryption. 
        public string KeyFilePath
        {
            get { return pKeyFilePath; }
        }


        // Gets the provider name. 
        public override string Name
        {
            get { return pName; }
        }


        // Performs provider initialization. 
        public override void Initialize(string name, NameValueCollection config)
        {
            pName = name;
            //            pKeyFilePath = config["keyContainerName"];
            ReadKey(KeyFilePath);
        }


        // Performs encryption. 
        public override XmlNode Encrypt(XmlNode node)
        {
            string encryptedData = EncryptString(node.OuterXml);

            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml("<EncryptedData>" + encryptedData + "</EncryptedData>");

            return xmlDoc.DocumentElement;
        }

        // Performs decryption. 
        public override XmlNode Decrypt(XmlNode encryptedNode)
        {
            string decryptedData = DecryptString(encryptedNode.InnerText);

            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(decryptedData);

            return xmlDoc.DocumentElement;
        }

        // Encrypts a configuration section and returns  
        // the encrypted XML as a string. 
        private string EncryptString(string encryptValue)
        {
            byte[] valBytes = Encoding.Unicode.GetBytes(encryptValue);

            var transform = des.CreateEncryptor();

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(valBytes, 0, valBytes.Length);
            cs.FlushFinalBlock();
            byte[] returnBytes = ms.ToArray();
            cs.Close();

            return Convert.ToBase64String(returnBytes);
        }


        // Decrypts an encrypted configuration section and  
        // returns the unencrypted XML as a string. 
        private string DecryptString(string encryptedValue)
        {
            byte[] valBytes = Convert.FromBase64String(encryptedValue);

            var transform = des.CreateDecryptor();

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(valBytes, 0, valBytes.Length);
            cs.FlushFinalBlock();
            byte[] returnBytes = ms.ToArray();
            cs.Close();

            return Encoding.Unicode.GetString(returnBytes);
        }

        // Generates a new TripleDES key and vector and  
        // writes them to the supplied file path. 
        public void CreateKey()
        {
            des.GenerateKey();
            des.GenerateIV();

            var sw = new StreamWriter(KeyFilePath, false);
            sw.WriteLine(ByteToHex(des.Key));
            sw.WriteLine(ByteToHex(des.IV));
            sw.Close();
        }


        // Reads in the TripleDES key and vector from  
        // the supplied file path and sets the Key  
        // and IV properties of the  
        // TripleDESCryptoServiceProvider. 
        private void ReadKey(string filePath)
        {
            var sr = new StreamReader(filePath);
            var keyValue = sr.ReadLine();
            var ivValue = sr.ReadLine();
            des.Key = HexToByte(keyValue);
            des.IV = HexToByte(ivValue);
        }


        // Converts a byte array to a hexadecimal string. 
        private string ByteToHex(byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
        }

        // Converts a hexadecimal string to a byte array. 
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

    }
}

