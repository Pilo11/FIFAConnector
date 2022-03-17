using System;
using System.Net;
using FIFAConnectorClient;

try
{
    if (args.Length != 1)
    {
        throw new ArgumentException("Did not find the target IP address, please input the target ip adress.");
    }
    IPAddress targetIp = IPAddress.Parse(args[0]);
    GameManager.Init(targetIp);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(Environment.NewLine + Environment.NewLine);
    Console.WriteLine(ex);
}

Console.ReadLine();