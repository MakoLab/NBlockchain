
using System;
using NBlockchain.P2PPrototocol.Network;

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
      Network.INetworkAgentAPI _newNetwork = new CommunicationEngine();
      m_Agent = new AgentAPI.AgentServices(Repository.BlockchainStore.Instance(), _newNetwork, Log);
      _newNetwork.initP2PServer();
    }
    private AgentAPI.AgentServices m_Agent;

  }
}
