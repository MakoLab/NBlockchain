
using System;

namespace NBlockchain.P2PPrototocol.Repository
{

  internal class NewBlockEventArgs : EventArgs
  {

    public NewBlockEventArgs(IBlock newBlock)
    {
      Block = newBlock;
    }
    public IBlock Block { get; private set; }

  }

}
