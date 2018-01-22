
using System;
using System.Reflection;
using NBlockchain.P2PPrototocol;

namespace NBlockchain.Host
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine($"Starting new node release {Assembly.GetExecutingAssembly().GetName().ToString()} ");
      Console.WriteLine($"Write new line to stop the application");
      string _cmdLine = Environment.CommandLine;
      string[] _parameters = Environment.GetCommandLineArgs();
      int _p2pWSPortNumber = -1;
      int _AgentHTTPServerPortNumber = -1;
      if (_parameters.Length >= 2 && int.TryParse(_parameters[1], out _p2pWSPortNumber))
        Console.WriteLine($"The WebSocket server will run on {_p2pWSPortNumber} port");
      else
        Console.WriteLine($"The WebSocket server will run on default port number");
      if (_parameters.Length >= 3 && int.TryParse(_parameters[2], out _AgentHTTPServerPortNumber))
        Console.WriteLine($"The Agent handling HTTP Server will run on {_AgentHTTPServerPortNumber} port");
      else
        Console.WriteLine($"The Agent handling HTTP Server will run on default port");
      using (Bootstrap _newNode = new Bootstrap())
      {
        _newNode.Log = message => Console.WriteLine(message);
        _newNode.Run(_p2pWSPortNumber, _AgentHTTPServerPortNumber);
        Console.ReadLine();
      }
    }
  }
}
