﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBlockchain.P2PPrototocol.lUnitTest
{
  [TestClass]
  public class CryptoJSUnitTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      string _sha = NodeJSAPI.CryptoJS.SHA256("Zażółć gęślą jaźń");
      Assert.AreEqual<string>("bc5348fd7c2dd8bbf411f0b9268265f7c2e0d31ebf314695882b8170c7e1e9d7".ToUpper(), _sha);
    }
  }
}