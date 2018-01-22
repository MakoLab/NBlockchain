
using System;
using System.Reflection;
using NBlockchain.P2PPrototocol;

namespace NBlockchain.Host
{
  class Program
  {
    static void Main(string[] args)
    {
      string _cmdLine = Environment.CommandLine;
      string[] _parameters = Environment.GetCommandLineArgs();

      using (Bootstrap _newNode = new Bootstrap())
      {
        Console.WriteLine($"Starting new node release {Assembly.GetExecutingAssembly().GetName().ToString()} ");
        Console.WriteLine($"Write new line to stop the application");
        _newNode.Log = message => Console.WriteLine(message);
        _newNode.Run();
        Console.ReadLine();
      }
    }
  }
}
