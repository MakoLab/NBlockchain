
using System;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol
{
  /// <summary>
  /// Bootstrap
  /// </summary>
  public class Bootstrap : IDisposable
  {
    public void Dispose()
    {
      m_Agent.Dispose();
    }
    internal Action<string> Log { get; set; } = message => { };
    /// <summary>
    /// Run the communication machine
    /// </summary>
    public void Run()
    {
      m_BlockchainStore = new BlockchainStore();
      Network.INetworkAgentAPI _newNetwork = new CommunicationEngine(m_BlockchainStore);
      m_Agent = new AgentAPI.AgentServices(m_BlockchainStore, _newNetwork, Log);
      _newNetwork.initP2PServer();
    }

    private BlockchainStore m_BlockchainStore;
    private AgentAPI.AgentServices m_Agent;


  }
}
