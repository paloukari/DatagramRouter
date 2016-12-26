using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antigonis8583OperationServerProxy.Antigonis8583OperationServer;
using AntigonisTypes;
using System.Transactions;

namespace Corp.TestAntigonisOperationClient
{
  class Program
  {
    const string 8583EStatementFlagQueryCommandValue = "8583EStatementFlagQuery";
    const string 8583EStatementFlagUpdateCommandValue = "8583EStatementFlagUpdate";
    const string 8583CardActivationCommandValue = "8583CardActivation";
    const string 8583CardCashLimitCommandValue = "8583CardCashLimit";
    const string 8583CardDeactivationCommandValue = "8583CardDeactivation";
    const string 8583CardPinReissuingCommandValue = "8583CardPinReissuing";

    const string menuMessage = "\n\nCommand: \n" +
      "1: " + 8583EStatementFlagQueryCommandValue + "\n" +
      "2: " + 8583EStatementFlagUpdateCommandValue + "\n" +
      "3: " + 8583CardActivationCommandValue + "\n" +
      "4: " + 8583CardCashLimitCommandValue + "\n" +
      "5: " + 8583CardDeactivationCommandValue + "\n" +
      "6: " + 8583CardPinReissuingCommandValue + "\n" +
      "0: EXIT\n" +
      "\n";

    static void Main(string[] args)
    {
      string userInput = string.Empty;
      bool exitFlag = false;


      Console.WriteLine("Starting..");

      while (exitFlag == false)
      {
        try
        {
          userInput = GetKey(menuMessage);
          if (string.IsNullOrWhiteSpace(userInput))
          {
            userInput = "1";
          }


          using (TransactionScope scope = new TransactionScope(new TransactionScopeOption() { }))
          {
            using (8583OperationClient client = new 8583OperationClient("Antigonis8583OperationServerEndPointOleTx"))
            {
              try
              {
                switch (userInput)
                {
                  case "1":
                    Execute8583EStatementFlagQuery(client);
                    break;
                  case "2":
                    Execute8583EStatementFlagUpdate(client);
                    break;
                  case "3":
                    Execute8583CardActivation(client);
                    break;
                  case "4":
                    Execute8583CardCashLimit(client);
                    break;
                  case "5":
                    Execute8583CardDeactivation(client);
                    break;
                  case "6":
                    Execute8583CardPinReissuing(client);
                    break;
                  case "0":
                    exitFlag = true;
                    break;
                  default:
                    break;
                }

                scope.Complete();
              }
              catch (Exception ex)
              {
                Console.WriteLine(ex.ToString());
              }
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
      Console.WriteLine("Exiting..");
    }


    public static void DisplayMenuMessage(string message)
    {
      if (string.IsNullOrWhiteSpace(message) == false)
      {
        Console.WriteLine(message);
      }
      else
      {
        Console.WriteLine(menuMessage);
      }
    }
    private static string GetKey(
       string message)
    {
      string result;

      while (Console.KeyAvailable)
      {
        Console.ReadKey(true);
      }

      DisplayMenuMessage(message);

#if WAIT_FOR_ENTER_KEY
      result = Console.ReadLine().ToUpper();
#else
      ConsoleKeyInfo ci = Console.ReadKey(true);
      result = ci.KeyChar.ToString().ToUpper();
#endif

      return result;
    }
    
    private static void Execute8583EStatementFlagQuery(8583OperationClient client)
    {
      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;

      var request = new 8583EStatementFlagQueryRequest();
      request.CreateAntigonisHeader("Corp-8583OPERATION", 8583EStatementFlagQueryCommandValue);
      request.AccountNumber = cardNumber;

      Console.WriteLine(request.SerializeToXML());

      var response = client.8583EStatementFlagQuery(request);

      if (response == null)
        Console.WriteLine("response == null");
      else
        Console.WriteLine(response.SerializeToXML());
    }

    private static void Execute8583EStatementFlagUpdate(8583OperationClient client)
    {
      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;

      string flagValue = "0";
      Console.WriteLine("Enter flag value(" + flagValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        flagValue = line;

      var request = new 8583EStatementFlagUpdateRequest();
      request.CreateAntigonisHeader("Corp-8583OPERATION", 8583EStatementFlagUpdateCommandValue);
      request.AccountNumber = cardNumber;
      request.EStatementFlag = int.Parse(flagValue);

      Console.WriteLine(request.SerializeToXML());

      var response = client.8583EStatementFlagUpdate(request);

      if (response == null)
        Console.WriteLine("response == null");
      else
        Console.WriteLine(response.SerializeToXML());
    }

    private static void Execute8583CardActivation(8583OperationClient client)
    {
      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;

      string expirationDateValue = "1234567890123456";
      Console.WriteLine("Enter expiration Date value(" + expirationDateValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        expirationDateValue = line;


      string channelValue = "1234567890123456";
      Console.WriteLine("Enter channel value(" + channelValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        channelValue = line;

      var request = new 8583CardActivationRequest();
      request.CreateAntigonisHeader("Corp-8583OPERATION", 8583CardActivationCommandValue);
      request.AccountNumber = cardNumber;
      request.CardExpirationDate = expirationDateValue;
      request.Channel = channelValue;

      Console.WriteLine(request.SerializeToXML());

      var response = client.8583CardActivation(request);

      if (response == null)
        Console.WriteLine("response == null");
      else
        Console.WriteLine(response.SerializeToXML());
    }
    private static void Execute8583CardCashLimit(8583OperationClient client)
    {
      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;

      string amountValue = "0";
      Console.WriteLine("Enter amount value(" + amountValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        amountValue = line;


      string channelValue = "WEB";
      Console.WriteLine("Enter channel value(" + channelValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        channelValue = line;

      var request = new 8583CardCashLimitRequest();
      request.CreateAntigonisHeader("Corp-8583OPERATION", 8583CardCashLimitCommandValue);
      request.AccountNumber = cardNumber;
      request.Amount = decimal.Parse(amountValue);
      request.Channel = channelValue;

      Console.WriteLine(request.SerializeToXML());

      var response = client.8583CardCashLimit(request);

      if (response == null)
        Console.WriteLine("response == null");
      else
        Console.WriteLine(response.SerializeToXML());
    }

    private static void Execute8583CardDeactivation(8583OperationClient client)
    {
      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;

      string flagValue = "0";
      Console.WriteLine("Enter flag value(" + flagValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        flagValue = line;

      var request = new 8583CardDeactivationRequest();
      request.CreateAntigonisHeader("Corp-8583OPERATION", 8583CardDeactivationCommandValue);
      request.AccountNumber = cardNumber;
      //TODO:fill the rest values here

      Console.WriteLine(request.SerializeToXML());

      var response = client.8583CardDeactivation(request);

      if (response == null)
        Console.WriteLine("response == null");
      else
        Console.WriteLine(response.SerializeToXML());
    }

    private static void Execute8583CardPinReissuing(8583OperationClient client)
    {
      string cardNumber = "5...........16";
      Console.WriteLine("Enter card number(" + cardNumber + ")");
      var line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        cardNumber = line;

      string flagValue = "0";
      Console.WriteLine("Enter flag value(" + flagValue + ")");
      line = Console.ReadLine();
      if (!string.IsNullOrEmpty(line))
        flagValue = line;

      var request = new 8583CardPinReissuingRequest();
      request.CreateAntigonisHeader("Corp-8583OPERATION", 8583CardPinReissuingCommandValue);
      request.AccountNumber = cardNumber;
      //TODO:fill the rest values here

      Console.WriteLine(request.SerializeToXML());

      var response = client.8583CardPinReissuing(request);

      if (response == null)
        Console.WriteLine("response == null");
      else
        Console.WriteLine(response.SerializeToXML());
    }
  }
}
