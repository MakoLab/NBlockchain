
using System;
using System.Collections.Generic;
using System.Linq;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal class BlockchainStore : IRepositoryAgentInterface, IRepositoryNetwork
  {

    internal BlockchainStore(Action<string> log)
    {
      blockchain.Add(getGenesisBlock());
      Log = log;
    }
    internal Action<string> Log { get; set; }

    #region IRepositoryAgentInterface
    public string stringify()
    {
      return blockchain.Stringify<List<Block>>();
    }
    public IBlock generateNextBlock(string blockData)
    {
      Block previousBlock = (Block)getLatestBlock();
      Block newBlock = new Block(previousBlock, blockData);
      Add(newBlock);
      Broadcast?.Invoke(this, new NewBlockEventArgs(newBlock));
      return newBlock;
    }
    #endregion

    #region IRepositoryNetwork
    public event EventHandler<NewBlockEventArgs> Broadcast;
    public int Count { get { return blockchain.Count; } }
    public IBlock getLatestBlock()
    {
      return blockchain[blockchain.Count - 1];
    }
    public void Add(IBlock newBlock)
    {
      blockchain.Add((Block)newBlock);
    }
    public bool isValidChain(IEnumerable<IBlock> blockchainToValidate)
    {
      List<Block> _blockchainToValidate = blockchainToValidate.Cast<Block>().ToList<Block>();
      if (_blockchainToValidate[0] != getGenesisBlock())
        return false;
      List<Block> tempBlocks = new List<Block>() { _blockchainToValidate[0] };
      for (int i = 1; i < _blockchainToValidate.Count; i++)
      {
        if (Block.isValidNewBlock(_blockchainToValidate[i], tempBlocks[i - 1]))
          tempBlocks.Add(_blockchainToValidate[i]);
        else
          return false;
      }
      return true;
    }

    public void handleBlockchainResponse(string data, Action queryAll)
    {
      List<Block> receivedBlocks = data.Parse<List<Block>>(); // JSON.parse(message.data).sort((b1, b2) => (b1.index - b2.index));
      receivedBlocks.Sort();
      Block latestBlockReceived = receivedBlocks[receivedBlocks.Count - 1];
      IBlock latestBlockHeld = getLatestBlock();
      if (latestBlockReceived.index > latestBlockHeld.index)
      {
        Log($"blockchain possibly behind. We got: {latestBlockHeld.index} Peer got: {latestBlockReceived.index}");
        if (latestBlockHeld.hash == latestBlockReceived.previousHash)
        {
          Log("We can append the received block to our chain");
          Add(latestBlockReceived);
          Broadcast?.Invoke(this, new NewBlockEventArgs( getLatestBlock()));
        }
        else if (receivedBlocks.Count == 1)
        {
          Log("We have to query the chain from our peer");
          queryAll();
        }
        else
        {
          Log("Received blockchain is longer than current blockchain");
          replaceChain(receivedBlocks);
        }
      }
      else
        Log("received blockchain is not longer than received blockchain. Do nothing");
    }
    private void replaceChain(List<Block> newBlocks)
    {
      if (isValidChain(newBlocks) && newBlocks.Count > Count)
      {
        Log("Received blockchain is valid. Replacing current blockchain with received blockchain");
        blockchain = newBlocks;
        Broadcast?.Invoke(this, new NewBlockEventArgs(getLatestBlock()));
      }
      else
        Log("Received blockchain invalid");
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
