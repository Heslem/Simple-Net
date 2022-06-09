using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SimpleNet
{
    /// <summary>
    /// Class for to process connected clients
    /// </summary>
    public sealed class Server
    {
        #region Variables
        public int CountConnections => _clients.Count;

        public Action<Client> ClientConnect { get; set; }
        public Action<Client> ClientDisconnect { get; set; }



        private readonly List<Client> _clients = new List<Client>();
        private Listener _listener;

        private static Server s_server;
        #endregion

        #region Constructors
        private Server()
        {
            
        }
        #endregion

        #region Metods

        /// <summary>
        /// Get server instance.
        /// </summary>
        public static Server GetServer()
        {
            if (s_server == null)
                s_server = new Server();
            return s_server;
        }

        public void Init(string ip = "127.0.0.1", int port = 8008)
        {
            _listener = new Listener(ip, port, this); 
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void AddConnection(TcpClient tcpClient)
        {
            Client client = new Client(tcpClient);
            _clients.Add(client);
            ClientConnect?.Invoke(client);
        }

        public void RemoveConnection(Client client)
        {
            ClientDisconnect?.Invoke(client);
            _clients.Remove(client);
        }

        /// <summary>
        /// Send all connected clients a packet.
        /// </summary>
        public void Broadcast(Packet packet, string id = "server")
        {
            for (ushort i = 0; i < _clients.Count; i++)
            {
                try
                {
                    if (_clients[i].ID != id) 
                        _clients[i].Send(packet);
                }
                catch (Exception exception)
                {
                    RemoveConnection(_clients[i]);
                    Console.WriteLine(exception.Message);
                }
            }
        }
        #endregion
    }
}
