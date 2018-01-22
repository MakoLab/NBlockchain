
using System;

namespace NBlockchain.P2PPrototocol.Repository
{

  internal interface IBlock : IEquatable<IBlock>, IComparable<IBlock>
  {

    int index { get; }
    string previousHash { get; }
    long timestamp { get; }
    string data { get; }
    string hash { get; }
    string stringify();

  }

}