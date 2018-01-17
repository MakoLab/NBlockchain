
namespace NBlockchain.P2PPrototocol.Repository
{
  internal interface IRepositoryAgentInterface
  {
    string stringify();
    Block generateNextBlock(string data);
  }
}
