using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Application
{
    public class ChatClient
    {
        public HashSet<string> AllClients = new HashSet<string>();
        private TcpClient _client;
        private string _clientIP;
        private string _clientNick;
        private byte[] data;
        private bool ReceiveNick = true;

        public ChatClient(TcpClient client)
        {
            _client = client;
            _clientIP = client.Client.RemoteEndPoint.ToString();
            AllClients.Add(_clientIP);
            data = new byte[_client.ReceiveBufferSize];
            Console.WriteLine("New Connection at: " + _clientIP);
            _client.GetStream().BeginRead(data, 0, data.Length, new AsyncCallback(ReceiveMessage), null);


        }

        public void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                lock (_client.GetStream())
                {
                    bytesRead = _client.GetStream().EndRead(ar);
                }

                //client has disconnected
                if (bytesRead < 1)
                {
                    AllClients.Remove(_clientIP);
                    Broadcast(_clientNick + " has left the chat.");
                    return;
                }
                else
                {
                    string messageReceived = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);

                    if (ReceiveNick)
                    {
                        _clientNick = messageReceived;
                        Broadcast(_clientNick + " has joined the chat.");
                        ReceiveNick = false;
                    }
                    else
                    {
                        Broadcast(_clientNick + " says:\n" + messageReceived);
                    }
                }

                lock (_client.GetStream())
                {
                    _client.GetStream().BeginRead(data, 0, _client.ReceiveBufferSize, ReceiveMessage, null);
                }

            }
            catch
            {
                AllClients.Remove(_clientIP);
                Broadcast(_clientNick + " has left the chat.");
            }

        }

        public void SendMessage(string message)
        {
            try
            {
                System.Net.Sockets.NetworkStream ns;
                lock (_client.GetStream())
                {
                    ns = _client.GetStream();
                }

                byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();
            }
            catch
            {
                Console.WriteLine("Error: could not send");
            }

        }

        public void Broadcast(string message)
        {
            Console.WriteLine(message);
            foreach (string c in AllClients)
            {
                SendMessage(message);
            }

        }


    }
}
