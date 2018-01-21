
using System;
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal class BlockchainStore : IRepositoryAgentInterface, IRepositoryNetwork
  {

    internal BlockchainStore()
    {
      blockchain.Add(getGenesisBlock());
    }

    internal class NewBlockEventArgs : EventArgs
    {
      public NewBlockEventArgs(Block newBlock)
      {
        Block = newBlock;
      }
      public Block Block { get; private set; }
    }

    #region IRepositoryAgentInterface
    public string stringify()
    {
      return blockchain.stringify<List<Block>>();
    }
    public Block generateNextBlock(string blockData)
    {
      Block previousBlock = getLatestBlock();
      Block newBlock = new Block(previousBlock, blockData);
      Add(newBlock);
      Broadcast?.Invoke(this, new NewBlockEventArgs(newBlock));
      return newBlock;
    }
    #endregion

    #region IRepositoryNetwork
    public event EventHandler<NewBlockEventArgs> Broadcast;
    public int Count { get { return blockchain.Count; } }
    public Block getLatestBlock()
    {
      return blockchain[blockchain.Count - 1];
    }
    public void Add(Block newBlock)
    {
      blockchain.Add(newBlock);
    }
    public bool isValidChain(List<Block> blockchainToValidate)
    {
      if (blockchainToValidate[0] != getGenesisBlock())
        return false;
      List<Block> tempBlocks = new List<Block>() { blockchainToValidate[0] };
      for (int i = 1; i < blockchainToValidate.Count; i++)
      {
        if (Block.isValidNewBlock(blockchainToValidate[i], tempBlocks[i - 1]))
          tempBlocks.Add(blockchainToValidate[i]);
        else
          return false;
      }
      return true;
    }
    public void replaceChain(List<Block> newBlocks)
    {
      blockchain = newBlocks;
    }
    #endregion

    #region private
    private static Block getGenesisBlock()
    {
      return new Block(0, "0", 1465154705, "my genesis block!!", "816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7");
    }
    private List<Block> blockchain { get; set; } = new List<Block>();
    #endregion

  }

}
