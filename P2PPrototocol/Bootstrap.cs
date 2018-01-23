
using System;
using System.Threading.Tasks;
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
    public async Task Run(int p2pPortNumber, int _AgentHTTPServerPortNumber)
    {
      try
      {
        m_BlockchainStore = new BlockchainStore(Log);
        m_CommunicationEngine = new CommunicationEngine(m_BlockchainStore, p2pPortNumber, Log);
        m_Agent = new AgentAPI.AgentServices(m_BlockchainStore, m_CommunicationEngine, _AgentHTTPServerPortNumber, Log);
        await m_CommunicationEngine.InitP2PServerAsync();
      }
      catch (Exception ex)
      {
        Log($"The aplikcation has been handled by the exception {ex}");
      }
    }

    private BlockchainStore m_BlockchainStore;
    private AgentServices m_Agent;
    private CommunicationEngine m_CommunicationEngine;


  }
}
