
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal class BlockchainStore
  {

    internal static BlockchainStore Instance()
    {
      return m_Singleton;
    }
    internal void Add(Block newBlock)
    {
      blockchain.Add(newBlock);
    }
    //internal void addBlock(Block newBlock)
    //{
    //  if (Block.isValidNewBlock(newBlock, getLatestBlock()))
    //    Add(newBlock);
    //}
    internal Block generateNextBlock(string blockData)
    {
      Block previousBlock = getLatestBlock();
      Block newBlock = new Block(previousBlock, blockData);
      Add(newBlock);
      return newBlock;
    }
    internal bool isValidChain(List<Block> blockchainToValidate)
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
    internal void replaceChain(List<Block> newBlocks)
    {
      blockchain = newBlocks;
    }
    internal Block getLatestBlock()
    {
      return blockchain[blockchain.Count - 1];
    }
    internal string stringify()
    {
      return JSON.stringify(blockchain);
    }
    internal int Count { get { return blockchain.Count; } }

    #region private
    private static BlockchainStore m_Singleton { get; } = new BlockchainStore();
    private List<Block> blockchain { get; set; } = new List<Block>();
    private static Block getGenesisBlock()
    {
      return new Block(0, "0", 1465154705, "my genesis block!!", "816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7");
    }
    private BlockchainStore()
    {
      blockchain.Add(getGenesisBlock());
    }
    #endregion

  }

}
