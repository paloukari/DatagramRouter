using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Corp.TestTcpClient
{
    internal class AsynchronousClient
    {
        
        //from http://msdn.microsoft.com/en-us/library/bew39x2a.aspx
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        static int _messages = 10000;


        static Random _random = new Random();
        private static int RandomMessageSize(int min, int max)
        {
            return _random.Next(min, max);
        }

        private static string RandomMessage(int size, bool lowerCase = true)
        {
            //StringBuilder builder = new StringBuilder();
            //Random random = new Random();
            //char ch;
            //for (int i = 0; i < size; i++)
            //{
            //    ch = Convert.ToChar('a' + i % 10);
            //    builder.Append(ch);
            //}
            string message = "02007024058000C0800216430252012713000021300000000010000000000013020120200000000028000000000000028978084004222{{5F8146EC-727E-4BD2-9021-69526855AB76}}00223300000000000280300002000857120317";
            //if (lowerCase)
            //    message = builder.ToString().ToLower();
            //else
            //    message = builder.ToString();

            message = String.Format("ISO5V0{0:0000}", message.Length) + message;

            return message;
        }


        internal static void SimpleRouterServiceTest(string portStr, string IP, string dataToSend, MessageType messageType, int messages, bool waitForServerResponse, int id, Action<TestResult> completionCallback)
        {
            //Log.WriteLine("Starting test..");

            TestResult result = new TestResult(id);

            try
            {

                _messages = messages;

                int port = Int32.Parse(portStr);
                //Log.WriteLine("Server Port =" + port);

                IPAddress ipAddress = null;
                if (!IPAddress.TryParse(IP, out ipAddress))
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(IP);
                    ipAddress = (from ip in ipHostInfo.AddressList
                                 where ip.AddressFamily == AddressFamily.InterNetwork
                                 select ip).First();
                }

                //Log.WriteLine("Server IP = " + ipAddress.ToString());
                
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Log.WriteLine("Connecting..");

                // Log.WriteLine("Connecting " + id);
                client.Connect(remoteEP);

                //Log.WriteLine("Connected " + id);

                result.Connected = true;

                int bytesatSent = 0;
                if (dataToSend != null)
                {
                    //Log.WriteLine("Sending data " + id);                    

                    for (int ii = 0; ii < _messages; ii++)
                    {
                        byte[] byteData = MessageGenerator.GetTransaction(messageType);                        

                        SocketError sockError;
                        int bytesSent = client.Send(byteData, 0, byteData.Length, 0, out sockError);

                        if (!(sockError == SocketError.Success && bytesSent == byteData.Length))
                            throw new Exception("Error??");
                        bytesatSent += bytesSent;

                    }

                    result.SentData = true;

                    //}
                    //string text = sb.ToString();
                    //byte[] byteData = IsoInternalMessageGenerator.GenerateMessage();//Encoding.ASCII.GetBytes(text);

                    //SocketError sockError;
                    //int bytesSent = client.Send(byteData, 0, byteData.Length, 0, out sockError);

                    //if (sockError == SocketError.Success && bytesSent == byteData.Length)
                    //  result.SentData = true;

                    //Log.WriteLine("Sent " + bytesSent + " bytes to server.");

                    if (waitForServerResponse)
                    {
                        StateObject obj = new StateObject() { SendDataLength = bytesatSent, send = null, messageType = messageType, messagesSent = _messages };
                        result.ServerResponse = Receive(client, obj);
                        obj.ReceiveDone.WaitOne();

                        //  Debug.Assert(obj.send.ToString() == obj.rec.ToString());
                        //Log.WriteLine("Received response " + id);
                    }
                }

                //Log.WriteLine("Closing socket..");
                if (client.Connected)
                    client.Shutdown(SocketShutdown.Both);
                client.Close();
                //Log.WriteLine("Socket closed");



            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(), LogType.Error);
            }
            finally { completionCallback(result); }

        }

        //private static void ConnectCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket client = (Socket)((object[])ar.AsyncState)[0];
        //        Action<TestResult> completionCallback = (Action<TestResult>)((object[])ar.AsyncState)[1];

        //        client.EndConnect(ar);
        //        Log.WriteLine("Socket connected to " + client.RemoteEndPoint.ToString());

        //        completionCallback(new TestResult() { Connected = true });
        //    }
        //    catch (Exception e)
        //    {
        //        Log.WriteLine(e.ToString(), LogType.Error);
        //    }
        //    finally
        //    {
        //        connectDone.Set();
        //    }
        //}

        private static string Receive(Socket client, StateObject state)
        {
            try
            {
                // Create the state object.

                state.workSocket = client;

                // Begin receiving the data from the remote device.
                //int bytesRead = 0;


                var s = client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                //(e) => {
                //    bytesRead = client.EndReceive(e);
                //    if (bytesRead == 0)
                //        ((StateObject)e.AsyncState).ReadyToCloseSocket=true;
                //    state.rec.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                //    receiveDone.Set();
                //}, state);
                state.ReceiveDone.WaitOne();



                // The response from the remote device.
                String response = String.Empty;
                // All the data has arrived; put it in response.
                if (state.rec.Length > 1)
                {
                    response = state.rec.ToString();
                }
                // Signal that all bytes have been received.
                // receiveDone.Set();
                return response;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(), LogType.Error);
            }
            return null;
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    string data = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    state.rec.Append(data);
                    state.receivedCounter += data.Length;



                    //  if (state.receivedCounter > 10000)
                    //  client.Disconnect(true);


                    switch (state.messageType)
                    {
                        case MessageType.ISO8583:
                            if (state.receivedCounter + state.messagesSent * (49 )== state.SendDataLength)
                            {
                                //Debug.WriteLine(state.receivedCounter + " == " + state.SendDataLength);
                                state.ReceiveDone.Set();
                                return;
                            } break;
                        case MessageType.ISOInternal:
                            if ((state.receivedCounter + state.messagesSent * 155 == state.SendDataLength)||
                                (state.receivedCounter + state.messagesSent * 0 == state.SendDataLength))
                            {
                                //Debug.WriteLine(state.receivedCounter + " == " + state.SendDataLength);
                                state.ReceiveDone.Set();
                                return;
                            } break;
                        case MessageType.Operation:
                            {
                                //Debug.WriteLine(data);
                                if (state.receivedCounter == state.SendDataLength)
                                {
                                    //state.rec.Replace("WEB008", Environment.NewLine);
                                    //Debug.WriteLine(state.rec.ToString());
                                    //Debug.WriteLine(state.receivedCounter + " == " + state.SendDataLength);
                                    state.ReceiveDone.Set();
                                    return;
                                }
                            } break; 
                        case MessageType.All:
                            {
                                //Debug.WriteLine(data);
                                if ((state.receivedCounter + state.messagesSent * (49+155) == state.SendDataLength) ||
                                    (state.receivedCounter + state.messagesSent * (49) == state.SendDataLength))
                                {
                                    //state.rec.Replace("WEB008", Environment.NewLine);
                                    //Debug.WriteLine(state.rec.ToString());
                                    //Debug.WriteLine(state.receivedCounter + " == " + state.SendDataLength);
                                    state.ReceiveDone.Set();
                                    return;
                                }
                            }break;
                        default:
                            break;
                    }

                    //else
                    //  Debug.WriteLine("!!" + state.receivedCounter + " != " + state.SendDataLength);

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        ReceiveCallback, state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    //if (state.rec.Length > 1)
                    //{
                    //    response = state.rec.ToString();
                    //}
                    // Signal that all bytes have been received.
                    //                    Debug.WriteLine("receivedCounter = " + state.receivedCounter);
                    state.ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(), LogType.Error);
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Log.WriteLine("Sent " + bytesSent + "bytes to server.");

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(), LogType.Error);
            }
        }
    }
}
