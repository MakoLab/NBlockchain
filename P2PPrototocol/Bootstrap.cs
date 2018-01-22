
using System;
using NBlockchain.P2PPrototocol.AgentAPI;
using NBlockchain.P2PPrototocol.Network;
using NBlockchain.P2PPrototocol.Repository;

namespace NBlockchain.P2PPrototocol
{
  /// <summary>
  /// Bootstrap
  /// </summary>
  public class Bootstrap : IDisposable
  {

    #region MyRegion
    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
      m_Agent.Dispose();
      m_CommunicationEngine.Dispose();
    }
    #endregion
    /// <summary>
    /// Log
    /// </summary>
    public Action<string> Log { private get; set; } = message => { };
    /// <summary>
    /// Run the communication machine
    /// </summary>
    public void Run(int p2pPortNumber, int _AgentHTTPServerPortNumber)
    {
      m_BlockchainStore = new BlockchainStore(Log);
      m_CommunicationEngine = new CommunicationEngine(m_BlockchainStore, p2pPortNumber, Log);
      m_Agent = new AgentAPI.AgentServices(m_BlockchainStore, m_CommunicationEngine, _AgentHTTPServerPortNumber, Log);
      m_CommunicationEngine.initP2PServer();
    }

    private BlockchainStore m_BlockchainStore;
    private AgentServices m_Agent;
    private CommunicationEngine m_CommunicationEngine;


  }
}
