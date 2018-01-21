
namespace NBlockchain.P2PPrototocol.Repository
{

  internal interface IRepositoryAgentInterface
  {
    string stringify();
    IBlock generateNextBlock(string data);
  }

}
