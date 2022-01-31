using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNetworking
{
    public sealed class Server
    {
        private static Server _server;

        private readonly List<Client> _clients;

        public int CountConnections => _clients.Count;

        public Action<Client> ClientConnect;
        public Action<Client> ClientDisconnect;

        private Server()
        {
            _clients = new List<Client>();
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

        public static Server GetServer()
        {
            if (_server == null)
                _server = new Server();
            return _server;
        }

        /// <summary>
        /// Send all connected clients a packet
        /// </summary>
        public void Broadcast(Packet packet, string id = "server")
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                try
                {
                    if (_clients[i].ID != id) 
                        _clients[i].Send(packet);
                }
                catch (Exception)
                {
                    RemoveConnection(_clients[i]);
                }
            }
        }
    }
}
