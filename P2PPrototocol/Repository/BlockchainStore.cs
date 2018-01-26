
using System;
using System.Collections.Generic;
using NBlockchain.P2PPrototocol.NodeJSAPI;

namespace NBlockchain.P2PPrototocol.Repository
{
  internal class BlockchainStore : IRepositoryAgentInterface, IRepositoryNetwork
  {

    internal BlockchainStore(Action<string> log)
    {
      m_Blockchain.Add(GenesisBlock);
      Log = log;
    }
    internal Action<string> Log { get; set; }

    #region IRepositoryAgentInterface
    public string stringify()
    {
      return m_Blockchain.Stringify<List<Block>>();
    }
    public IBlock generateNextBlock(string blockData)
    {
      Block previousBlock = m_Blockchain[m_Blockchain.Count - 1];
      Block newBlock = new Block(previousBlock, blockData);
      m_Blockchain.Add(newBlock);
      Broadcast?.Invoke(this, new NewBlockEventArgs(newBlock));
      return newBlock;
    }
    #endregion

    #region IRepositoryNetwork
    public event EventHandler<NewBlockEventArgs> Broadcast;
    public string getLatestBlock()
    {
      List<Block> _lastBlockToSerialize = new List<Block>() { m_Blockchain[m_Blockchain.Count - 1] };
      return _lastBlockToSerialize.Stringify<List<Block>>();
    }
    public void handleBlockchainResponse(string data, Action queryAll)
    {
      List<Block> _receivedBlocks = data.Parse<List<Block>>(); // JSON.parse(message.data).sort((b1, b2) => (b1.index - b2.index));
      _receivedBlocks.Sort();
      Block _latestBlockReceived = _receivedBlocks[_receivedBlocks.Count - 1];
      Block _latestBlockHeld = m_Blockchain[m_Blockchain.Count - 1];
      if (_latestBlockReceived.index > _latestBlockHeld.index)
      {
        Log($"blockchain possibly behind. We got: {_latestBlockHeld.index} Peer got: {_latestBlockReceived.index}");
        if (_latestBlockHeld.hash == _latestBlockReceived.previousHash)
        {
          Log("We can append the received block to our chain");
          m_Blockchain.Add(_latestBlockReceived);
          Broadcast?.Invoke(this, new NewBlockEventArgs(m_Blockchain[m_Blockchain.Count - 1]));
        }
        else if (_receivedBlocks.Count == 1)
        {
          Log("We have to query the chain from our peer");
          queryAll();
        }
        else
        {
          Log("Received blockchain is longer than current blockchain");
          ReplaceChain(_receivedBlocks);
        }
      }
      else
        Log("received blockchain is not longer than received blockchain. Do nothing");
    }
    #endregion

    #region private
    private List<Block> m_Blockchain { get; set; } = new List<Block>();
    private static Block GenesisBlock => new Block(0, "0", 1465154705, "my genesis block!!", "816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7");
    private void ReplaceChain(List<Block> newBlocks)
    {
      if (IsValidChain(newBlocks) && newBlocks.Count > m_Blockchain.Count)
      {
        Log("Received blockchain is valid. Replacing current blockchain with received blockchain");
        m_Blockchain = newBlocks;
        Broadcast?.Invoke(this, new NewBlockEventArgs(m_Blockchain[m_Blockchain.Count - 1]));
      }
      else
        Log("Received invalid blockchain ");
    }
    private bool IsValidChain(List<Block> blockchainToValidate)
    {
      if (blockchainToValidate[0] != GenesisBlock)
        return false;
      List<Block> _tempBlocks = new List<Block>() { blockchainToValidate[0] };
      for (int i = 1; i < blockchainToValidate.Count; i++)
      {
        if (Block.isValidNewBlock(blockchainToValidate[i], _tempBlocks[i - 1]))
          _tempBlocks.Add(blockchainToValidate[i]);
        else
          return false;
      }
      return true;
    }
    #endregion

  }

}
