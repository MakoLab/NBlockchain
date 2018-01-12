
using System;
using System.Security.Cryptography;
using System.Text;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  internal static class CryptoJS
  {
    internal static string SHA256(string inputStream)
    {
      byte[] _inputStreamBytes = Encoding.UTF8.GetBytes(inputStream);
      using (SHA256 mySHA256 = SHA256Managed.Create())
      {
        byte[] hashValue = mySHA256.ComputeHash(_inputStreamBytes);
        return BitConverter.ToString(hashValue);
      }
    }
  }

}