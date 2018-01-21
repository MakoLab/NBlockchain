
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.lUnitTest.NodeJSAPI
{
  [TestClass]
  public class JSONUnitTest
  {

    [TestMethod]
    public void MessageSerializationTestMethod()
    {
      Message _message = new Message() { data = "test", type = Message.MessageType.QUERY_LATEST };
      string _result = _message.stringify();
      Assert.AreEqual<string>(_referenceMessage, _result);
    }
    [TestMethod]
    public void MessageDeserializationTestMethod()
    {
      Message _message = _referenceMessage.parse<Message>();
      Assert.AreEqual<string>(_message.data, "test");
      Assert.AreEqual<Message.MessageType>(_message.type,  Message.MessageType.QUERY_LATEST);
    }

    private readonly string _referenceMessage = "{\"data\":\"test\",\"type\":0}";

  }
}
