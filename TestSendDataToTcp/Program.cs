using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Sockets;

namespace TestSendDataToTcp
{
  class Program
  {
    static void Main(string[] args)
    {
      Test1();
    }


    private static void Test1()
    {
      try
      {
        string dataString;
        byte[] dataBytes;
        byte[] dataLengthBytes;

        Console.WriteLine("Press <Enter> to start");
        Console.ReadLine();

        string serverIp = ConfigurationManager.AppSettings["serverIp"];
        int serverPort = Int32.Parse(ConfigurationManager.AppSettings["serverPort"]);

        byte[] header = new byte[] { 0x60, 0x00, 0x22, 0x00, 0x00};
        dataString = "PHRQ123456789012345678901234567890123456789";
        dataBytes = System.Text.Encoding.GetEncoding(1253).GetBytes(dataString);
          
        //dataLengthBytes = BitConverter.GetBytes((UInt16)(dataBytes.Length));

        Console.WriteLine("Server IP: " + serverIp);
        Console.WriteLine("Server Port: " + serverPort);

        Console.WriteLine("Connecting...");
        TcpClient client = new TcpClient(serverIp, serverPort);
        Console.WriteLine("Connected.");

        NetworkStream stream = client.GetStream();
        //stream.Write(dataLengthBytes, 0, 2);
        //stream.WriteByte(dataLengthBytes[1]);
        //stream.WriteByte(dataLengthBytes[0]);
          
        stream.Write(header, 0, header.Length);
        stream.Write(dataBytes, 0, dataBytes.Length);
        stream.Flush();

        Console.WriteLine("Sent: [{0}]", dataString);

        Console.WriteLine("Press <Enter> to disconnect");
        Console.ReadLine();
        Console.WriteLine("Disconnecting...");
        client.Close();
        Console.WriteLine("Disconnected.");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }

      Console.WriteLine("\nPress any key to exit...");
      Console.ReadKey();
    }
  }


}
