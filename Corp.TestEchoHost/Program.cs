using System;
using System.Diagnostics;
using EchoHostServerLibrary;

namespace Corp.TestEchoHost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                EchoHostServer echoHost = new EchoHostServer();

                echoHost.Start();

                Console.WriteLine("Echo Host created");
                Console.WriteLine("Press <Enter> to exit");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
    }
}
