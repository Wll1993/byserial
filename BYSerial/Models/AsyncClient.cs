using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BYSerial.Models
{
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class AsyncClient
    {
        private int _Port;
        private string _IP;
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        #region 事件
        /// <summary>
        /// 收到新消息委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void NewMsgArrivedEventHandler(object sender, NewMsgArrivedEventArgs e);
        /// <summary>
        /// 收到新消息事件
        /// </summary>
        public event NewMsgArrivedEventHandler NewMsgArrived;

        public class NewMsgArrivedEventArgs : EventArgs
        {
            public readonly byte[] msg;            
            public NewMsgArrivedEventArgs(byte[] msg)
            {
                this.msg = msg;
            }
        }
        public delegate void ConnectedEventHandler(object sender,EventArgs e);
        public event ConnectedEventHandler Connected;
        public delegate void ClosedEventHandler(object sender, EventArgs e);
        public event ClosedEventHandler Closed;

        #endregion
        // The response from the remote device.  
        private String response = String.Empty;
        public bool IsConnected { get; private set; } = false;
        public Socket Client { get; private set; }

        public AsyncClient(string remoteip, int port)
        {
            _IP = remoteip;
            _Port = port;
        }
        private void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Parse(_IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, _Port);
                // Create a TCP/IP socket.  
                Client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                // Connect to the remote endpoint.  
                Client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), Client);
                //connectDone.WaitOne();

                //// Convert the string data to byte data using ASCII encoding.  
                //byte[] byteData = Encoding.ASCII.GetBytes("This is a test<EOF>");
                //// Send test data to the remote device.  
                //Send(byteData);

                //// Write the response to the console.  
                //Console.WriteLine("Response received : {0}", response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public bool Connect()
        {
            try
            {
                StartClient();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public bool Close()
        {
            try
            {
                // Release the socket.  
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
                if (Closed != null)
                {
                    Closed(Client, new EventArgs());
                    IsConnected = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);
                if(Connected!=null)
                {
                    Connected(this,new EventArgs());
                    IsConnected = true;
                }
                //Console.WriteLine("Socket connected to {0}",
                //    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private  void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private  void ReceiveCallback(IAsyncResult ar)
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
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                        if(NewMsgArrived!=null)
                        {
                            byte[] msg=Encoding.ASCII.GetBytes(response);
                            NewMsgArrivedEventArgs e = new NewMsgArrivedEventArgs(msg);
                            NewMsgArrived(this, e);
                        }
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(byte[] byteData)
        {
            // Begin sending the data to the remote device.  
            Client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), Client);
            sendDone.WaitOne();
            // Receive the response from the remote device.  
            Receive(Client);
            receiveDone.WaitOne();
        }

        private  void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        
    }
}
