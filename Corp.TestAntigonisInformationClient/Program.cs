using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.TestAntigonisInformationClient
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Starting..");

      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;
      
        using (InformationServiceReference.InformationServerClient client = new InformationServiceReference.InformationServerClient())
        {
          try
          {
            var response = client.ProcessInformationRequest(new InformationServiceReference.InformationMessageRequest() {  });

            Console.WriteLine(response.FullMessageText);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.ToString());
          }
        }
      
      Console.ReadLine();
      Console.WriteLine("Exiting..");
    }
  }
}
