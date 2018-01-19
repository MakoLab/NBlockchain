
using System.Security.Cryptography;
using System.Text;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  internal static class CryptoJS
  {
    internal static string SHA256(this string inputStream)
    {
      byte[] _inputStreamBytes = Encoding.UTF8.GetBytes(inputStream);
      using (SHA256 mySHA256 = SHA256Managed.Create())
      {
        byte[] hashValue = mySHA256.ComputeHash(_inputStreamBytes);
        return hashValue.ToHexString();
      }
    }
    private static string ToHexString(this byte[] bytes)
    {
      char[] c = new char[bytes.Length * 2];
      int b;
      for (int i = 0; i < bytes.Length; i++)
      {
        b = bytes[i] >> 4;
        c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
        b = bytes[i] & 0xF;
        c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
      }
      return new string(c);
    }

  }

}