
using System.Runtime.Serialization;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.AgentAPI
{
  [DataContract]
  public class DataContract
  {
    [DataMember]
    public string data { get; set; }

    internal static DataContract Parse(string body)
    {
      DataContract _data = body.parse<DataContract>();
      return _data;
    }
  }
}
