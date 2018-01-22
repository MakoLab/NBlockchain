
using System;
using System.Runtime.Serialization;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Repository
{

  [DataContract]
  internal class Block: IBlock
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

    #region IBlock
    [DataMember]
    public int index { get; private set; }
    [DataMember]
    public string previousHash { get; private set; }
    [DataMember]
    public long timestamp { get; private set; }
    [DataMember]
    public string data { get; private set; }
    [DataMember]
    public string hash { get; private set; }
    public string calculateHash()
    {
      return CryptoJS.SHA256($"{index}{previousHash}{timestamp}{data}");
    }
    public string stringify()
    {
      return this.Stringify<Block>();
    }
    #endregion

    #region constructors
    internal Block(int index, string previousHash, int timestamp, string data, string hash)
    {
      this.index = index;
      this.previousHash = previousHash;
      this.timestamp = timestamp;
      this.data = data;
      this.hash = hash;
    }
    internal Block(IBlock previousBlock, string blockData)
    {
      this.index = previousBlock.index + 1;
      this.previousHash = previousBlock.hash;
      this.timestamp = DateTime.Now.Timestamp(); // new Date().getTime() / 1000;
      this.data = blockData;
      this.hash = calculateHash();
    }
    #endregion

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
    #endregion

    #region IEquatable
    public bool Equals(IBlock other)
    {
      return Equals(other);
    }
    #endregion

    #region IComparable
    public int CompareTo(IBlock other)
    {
      return index.CompareTo(other.index);
    }

    #endregion

  }

}

