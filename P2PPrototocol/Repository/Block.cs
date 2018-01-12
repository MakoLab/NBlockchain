
using System;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal class Block
  {

    #region operators
    public static bool operator ==(Block one, Block second)
    {
      return one.Equals(second);
    }
    public static bool operator !=(Block one, Block second)
    {
      return !one.Equals(second);
    }
    internal static bool isValidNewBlock(Block newBlock, Block previousBlock)
    {
      if (previousBlock.index + 1 != newBlock.index)
      {
        //log("invalid index");
        return false;
      }
      else if (previousBlock.hash != newBlock.previousHash)
      {
        //log("invalid previoushash");
        return false;
      }
      else if (newBlock.calculateHash() != newBlock.hash)
      {
        //log($"{newBlock.hash} {newBlock.calculateHash()}");
        //log($"invalid hash: {newBlock.calculateHash()} {newBlock.hash}");
        return false;
      }
      return true;
    }
    #endregion

    internal int index { get; private set; }
    internal string previousHash { get; private set; }
    internal int timestamp { get; private set; }
    internal string data { get; private set; }
    internal string hash { get; private set; }

    internal Block(int index, string previousHash, int timestamp, string data, string hash)
    {
      this.index = index;
      this.previousHash = previousHash;
      this.timestamp = timestamp;
      this.data = data;
      this.hash = hash;
    }
    public Block(Block previousBlock, string blockData)
    {
      this.index = previousBlock.index + 1;
      this.previousHash = previousBlock.hash;
      this.timestamp = DateTime.Now.Timestamp(); // new Date().getTime() / 1000;
      this.data = blockData;
      this.hash = calculateHash();
    }
    internal string calculateHash()
    {
      return CryptoJS.SHA256($"{index}{previousHash}{timestamp}{data}");
    }
    #region override Object
    public override string ToString()
    {
      return $"{index}{previousHash}{timestamp}{data}";
    }
    public override int GetHashCode()
    {
      return ToString().GetHashCode();
    }
    public override bool Equals(object obj)
    {
      Block _yours = obj as Block;
      if (_yours == null)
        return false;
      return _yours.calculateHash() == this.calculateHash();
    }
    internal string stringify()
    {
      return JSON.stringify(this);
    }
    #endregion

  }

}

