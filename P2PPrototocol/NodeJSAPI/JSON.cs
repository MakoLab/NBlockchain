

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace NBlockchain.P2PPrototocol.NodeJSAPI
{

  /// <summary>
  /// Provides simple JSON serialization to <see cref="string"/>
  /// </summary>
  internal static class JSON
  {
    internal static type parse<type>(this string message)
    {
      if (String.IsNullOrEmpty(message))
        throw new ArgumentException(nameof(message), "The string cannot be null or empty.");
      DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(type));
      byte[] _buffer = Encoding.Default.GetBytes(message);
      using (MemoryStream _streem = new MemoryStream(_buffer))
      {
        return (type)_jsonSerializer.ReadObject(_streem);
      }
    }

    internal static string stringify<type>(this type graph)
    {
      if (graph == null)
        throw new ArgumentNullException(nameof(graph));
      DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(type));
      Formatting _formatting = new Formatting() { };
      using (MemoryStream _streem = new MemoryStream())
      {
        _jsonSerializer.WriteObject(_streem, graph);
        _streem.Flush();
        return Encoding.Default.GetString((_streem.ToArray()));
      }
    }

  }
}
