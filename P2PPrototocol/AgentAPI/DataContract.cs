
using System.Runtime.Serialization;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  /// <summary>
  /// class DataContract
  /// </summary>
  [DataContract]
  public class DataContract
  {

    /// <summary>
    /// data recovered form the network JSON string;
    /// </summary>
    [DataMember]
    public string data { get; set; }
    /// <summary>
    /// Parses the <paramref name="body"/> to recover an instance of <see cref="DataContract"/>
    /// </summary>
    /// <param name="body"></param>
    /// <returns>An instance of <see cref="DataContract"/> </returns>
    internal static DataContract Parse(string body)
    {
      DataContract _data = body.Parse<DataContract>();
      return _data;
    }

  }
}
