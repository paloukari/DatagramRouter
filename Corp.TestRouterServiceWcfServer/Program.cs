using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using Corp.OperationServiceProxy.OperationServiceReference;
using AntigonisTypes.Information;

using AntigonisHsmServerProxy.HsmServerProxy;
using Antigonis8583InformationServerProxy.Antigonis8583InformationServer;
using Corp.TestRouterServiceWcfServer.RouterServiceProxy;
using AntigonisTypes.ThreePleLayerSecurity;


namespace Corp.TestRouterServiceWcfServer
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("(3)PleLayerSecurity, (H)sm (O)peration or (I)nformation?(3/H/O/I)");
      string data = Console.ReadLine();
      switch (data.ToLower())
      {
        case "3":
          ThreePleLayerSecurityTest();
          break;
        case "h":
          HsmTest();
          break;
        case "o":
          OperationTest();
          break;
        case "i":
          InformationTest();
          break;
      }
    }

    private static void InformationTest()
    {
      string card = "4060012579914007";

      Console.WriteLine("(P)ing or (C)ard Summary?(P/C)");
      string data = Console.ReadLine();
      if (data.ToLower() == "p")
      {
        int iterations = 1;
        while (true)
        {
          Console.WriteLine("Enter number of iterations (" + iterations + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            if (!Int32.TryParse(data, out iterations))
            {
              break;
            }
          }

          for (int i = 0; i < iterations; i++)
          {
            try
            {
              8583InformationClient client = new 8583InformationClient();
              {


                client.BeginPing("Ping", (e) =>
                {
                  try
                  {
                    Console.WriteLine(client.EndPing(e).ToString());
                  }
                  catch (Exception inExc)
                  {
                    Console.WriteLine("!!!!!!!!EXCEPTION!!!!!!!!");
                    Console.WriteLine(inExc.ToString());
                  }
                }, null);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.ToString());
            }
          }

          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
      else if (data.ToLower() == "c")
      {
        int iterations = 1;
        while (true)
        {
          Console.WriteLine("Enter number of iterations (" + iterations + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            if (!Int32.TryParse(data, out iterations))
            {
              break;
            }
          }
          Console.WriteLine("Card number (" + card + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            card = data;
          }

          for (int i = 0; i < iterations; i++)
          {
            try
            {
              8583InformationClient client = new 8583InformationClient();
              {
                var request = new AntigonisTypes.8583CardSummaryQueryRequest() { AccountNumber = card };

                request.CreateAntigonisHeader("ALPHA-Corp", "8583EStatementCardSummaryQuery");

                client.BeginCardSummaryQuery(request, (e) =>
                {
                  try
                  {
                    var result = client.EndCardSummaryQuery(e);

                    Console.WriteLine("Account=" + result.AccountFormatted);
                    Console.WriteLine("Addon1Account=" + result.Addon1AccountFormatted);
                    Console.WriteLine("Addon1Name=" + result.Addon1NameFormatted);
                    Console.WriteLine("Addon2Account=" + result.Addon2AccountFormatted);
                    Console.WriteLine("Addon2Name=" + result.Addon2NameFormatted);
                    Console.WriteLine("ApplicationReferenceNumber=" + result.ApplicationReferenceNumberFormatted);
                    Console.WriteLine("BankAccount=" + result.BankAccountFormatted);
                    Console.WriteLine("BlackListCode=" + result.BlackListCodeFormatted);
                    Console.WriteLine("CDINumber=" + result.CDINumberFormatted);
                    Console.WriteLine("CreditLimit=" + result.CreditLimitFormatted);
                    Console.WriteLine("CurrencyCode=" + result.CurrencyCodeFormatted);
                    Console.WriteLine("CurrencyDecimals=" + result.CurrencyDecimalsFormatted);
                    Console.WriteLine("CurrencyLiteral=" + result.CurrencyLiteralFormatted);
                    //χαχαχαχαχαχα
                    Console.WriteLine("CurrentBanalance=" + result.CurrentBalanceFormatted);
                    Console.WriteLine("DateOfBirth=" + result.DateOfBirthFormatted);
                    Console.WriteLine("Error=" + result.Error);
                    Console.WriteLine("ExpirationDate=" + result.ExpirationDateFormatted);
                    Console.WriteLine("FirstName=" + result.FirstNameFormatted);
                    Console.WriteLine("IdNumber=" + result.IdNumberFormatted);
                    Console.WriteLine("IsSuccess=" + result.IsSuccess);
                    Console.WriteLine("LastName=" + result.LastNameFormatted);
                    Console.WriteLine("LastPayAmount=" + result.LastPayAmountFormatted);
                    Console.WriteLine("LastPayDate=" + result.LastPayDateFormatted);
                    Console.WriteLine("MemberSince=" + result.MemberSinceFormatted);
                    Console.WriteLine("MinimumPayAmount=" + result.MinimumPayAmountFormatted);
                    Console.WriteLine("MinimumPayDate=" + result.MinimumPayDateFormatted);
                    Console.WriteLine("OpenToBuy=" + result.OpenToBuyFormatted);
                    //Console.WriteLine("" + result.AntigonisHeader);
                    Console.WriteLine("PassportNumber=" + result.PassportNumberFormatted);
                    Console.WriteLine("ResponseCode=" + result.ResponseCode);
                    Console.WriteLine("ResponseDescription=" + result.ResponseDescription);
                    Console.WriteLine("SpendingLimit=" + result.SpendingLimitFormatted);
                    Console.WriteLine("StatementBalance=" + result.StatementBalanceFormatted);
                    Console.WriteLine("StatementDate=" + result.StatementDateFormatted);
                    Console.WriteLine("StdType=" + result.StdTypeFormatted);
                    Console.WriteLine("TotalAuths=" + result.TotalAuthsFormatted);
                    Console.WriteLine("DeliquentDays=" + result.DeliquentDaysFormatted);

                  }
                  catch (Exception inExc)
                  {
                    Console.WriteLine("!!!!!!!!EXCEPTION!!!!!!!!");
                    Console.WriteLine(inExc.ToString());
                  }
                }, null);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.ToString());
            }
          }

          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
    }

    private static void OperationTest()
    {
      Console.WriteLine("(P)ing or (V)alidate CVV?(P/V)");
      string data = Console.ReadLine();
      if (data.ToLower() == "p")
      {
        int iterations = 1;
        while (true)
        {
          Console.WriteLine("Enter number of iterations (" + iterations + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            if (!Int32.TryParse(data, out iterations))
            {
              break;
            }
          }

          for (int i = 0; i < iterations; i++)
          {
            try
            {
              OperationServiceClient client = new OperationServiceClient();
              {
                client.BeginPing(new OperationPingRequest() { }, (e) =>
                {
                  try
                  {
                    Console.WriteLine(client.EndPing(e).ToString());
                  }
                  catch (Exception inExc)
                  {
                    Console.WriteLine("!!!!!!!!EXCEPTION!!!!!!!!");
                    Console.WriteLine(inExc.ToString());
                  }
                }, null);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.ToString());
            }
          }

          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
      else
      {
        int iterations = 1;
        while (true)
        {
          Console.WriteLine("Enter number of iterations (" + iterations + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            if (!Int32.TryParse(data, out iterations))
            {
              break;
            }
          }

          for (int i = 0; i < iterations; i++)
          {
            try
            {
              OperationServiceClient client = new OperationServiceClient();
              {
                client.BeginGetCardSum(new OperationGetCardSumRequest() { }, (e) =>
                {
                  try
                  {
                    Console.WriteLine(client.EndGetCardSum(e).ToString());
                  }
                  catch (Exception inExc)
                  {
                    Console.WriteLine("!!!!!!!!EXCEPTION!!!!!!!!");
                    Console.WriteLine(inExc.ToString());
                  }
                }, null);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.ToString());
            }
          }

          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
    }

    private static void HsmTest()
    {
      Console.WriteLine("(P)ing or (V)alidate CVV?(P/V)");
      string data = Console.ReadLine();
      if (data.ToLower() == "v")
      {
        string card = "4060012579914007";
        string cvv = "059";
        string expirationDate = "0318";

        while (true)
        {
          try
          {
            Console.WriteLine("Enter card number (" + card + ")");
            data = Console.ReadLine();
            if (!string.IsNullOrEmpty(data))
              card = data;



            Console.WriteLine("Enter card expirationDate (" + expirationDate + ")");
            data = Console.ReadLine();
            if (!string.IsNullOrEmpty(data))
              expirationDate = data;

            Console.WriteLine("Find or check cvv (F/C)");
            if (Console.ReadLine().ToLower() == "f")
            {
              for (int i = 0; i <= 999; i++)
              {
                //Thread.Sleep(50);
                HsmClient client = new HsmClient();
                {

                  string cvvc = String.Format("{0:000}", i);
                  var request = new AntigonisTypes.ValidateCvv2QueryRequest()
                  {
                    CardNumber = card,
                    Cvv2 = cvvc,
                    ExpirationDate = expirationDate
                  };
                  request.CreateAntigonisHeader("ALPHA-Corp", "ValidateCvv2Query");
                  client.BeginValidateCvv2Query(request, e =>
                  {
                    var result = client.EndValidateCvv2Query(e);
                    if (result.IsValid)
                    {
                      Console.WriteLine(cvvc + " is Valid");
                    }
                    else
                    {
                      Console.WriteLine(cvvc + " is Invalid");
                    }
                  }, null);


                }
              }
            }
            else
            {
              Console.WriteLine("Enter card cvv (" + cvv + ")");
              data = Console.ReadLine();
              if (!string.IsNullOrEmpty(data))
                cvv = data;

              using (HsmClient client = new HsmClient())
              {
                var request = new AntigonisTypes.ValidateCvv2QueryRequest()
                {
                  CardNumber = card,
                  Cvv2 = cvv,
                  ExpirationDate = expirationDate
                };
                request.CreateAntigonisHeader("ALPHA-Corp", "ValidateCvv2Query");
                var result = client.ValidateCvv2Query(request);
                if (result.IsValid)
                  Console.WriteLine(" is Valid!!!!!!!!!");
                else
                  Console.WriteLine(" is Invalid :-(");
              }
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.ToString());
          }
          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
      else
      {
        int iterations = 1;
        while (true)
        {
          Console.WriteLine("Enter number of iterations (" + iterations + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            if (!Int32.TryParse(data, out iterations))
            {
              break;
            }
          }

          for (int i = 0; i < iterations; i++)
          {
            try
            {
              HsmClient client = new HsmClient();
              {
                var request = new AntigonisTypes.HsmDiagnosticRequest() { };
                request.CreateAntigonisHeader("Corp-Corp", "HsmDiagnostic");

                client.BeginHsmDiagnostic(request, (e) =>
                {
                  try
                  {
                    var response = client.EndHsmDiagnostic(e);
                    Console.WriteLine("BufferSize=" + response.BufferSize);
                    Console.WriteLine("EthernetType=" + response.EthernetType);
                    Console.WriteLine("FirmwareNumber=" + response.FirmwareNumber);
                    Console.WriteLine("NumberOfTcpSockets=" + response.NumberOfTcpSockets);
                  }
                  catch (Exception inExc)
                  {
                    Console.WriteLine("!!!!!!!!EXCEPTION!!!!!!!!");
                    Console.WriteLine(inExc.ToString());
                  }
                }, null);
              }
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.ToString());
            }
          }

          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
      return;
    }

    private static void ThreePleLayerSecurityTest()
    {
      Console.WriteLine("(P)ing or (E)xecute ?(P/E)");
      string data = Console.ReadLine();
      if (data.ToLower() == "e")
      {
        string messageBody = "WEB007002  7851C43368C00BBC943705F7F5590C18";

        while (true)
        {
          try
          {
            Console.WriteLine("Enter card number (" + messageBody + ")");
            data = Console.ReadLine();
            if (!string.IsNullOrEmpty(data))
              messageBody = data;


            using (ThreePleLayerSecurityServerClient client = new ThreePleLayerSecurityServerClient())
            {
              var request = new ThreePleLayerSecurityMessageRequest()
              {
                MessageBody = messageBody
              };
              //request.CreateAntigonisHeader("ALPHA-Corp", "ValidateCvv2Query");
              var result = client.ProcessThreePleLayerSecurityRequest(request);
              if (result.IsValid)
              {
                Console.WriteLine(" is Valid!!!!!!!!!");
                Console.WriteLine(result.MessageBody);
              }
              else
                Console.WriteLine(" is Invalid :-(");
              
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.ToString());
          }
          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
      else
      {
        int iterations = 1;
        while (true)
        {
          Console.WriteLine("Enter number of iterations (" + iterations + ")");
          data = Console.ReadLine();
          if (!string.IsNullOrEmpty(data))
          {
            if (!Int32.TryParse(data, out iterations))
            {
              break;
            }
          }

          for (int i = 0; i < iterations; i++)
          {
            try
            {
              ThreePleLayerSecurityServerClient client = new ThreePleLayerSecurityServerClient();
              {
                var request = new AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageRequest() { };
                //request.CreateAntigonisHeader("Corp-Corp", "HsmDiagnostic");

                var res2 = client.Ping(request);
                Console.WriteLine("Response=" + res2);

              }
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex.ToString());
            }
          }

          Console.WriteLine("Continue?(Y/N)");
          string res = Console.ReadLine();
          if (res.ToLower() == "n")
          {
            Console.WriteLine("Exiting..");
            break;
          }
        }
      }
      return;
    }



  }
}
