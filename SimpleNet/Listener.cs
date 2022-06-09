using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleNet
{
    internal sealed class Listener
    {
        private readonly TcpListener _listener;
        private readonly Thread _listenThread;

        private bool _working = false;

        private readonly Server _server;

        public Listener(string ip, int port, Server server)
        {
            _listener = new TcpListener(IPAddress.Parse(ip), port);
            _listenThread = new Thread(new ThreadStart(Listen));
            _server = server;
        }

        /// <summary>
        /// Start listener
        /// </summary>
        public void Start()
        {
            _working = true;

            _listener.Start();
            _listenThread.Start();
        }

        /// <summary>
        /// Stop listener
        /// </summary>
        public void Stop()
        {
            _working = false;
            _listener.Stop();
        }

        private void Listen()
        {
            while (_working)
            {
                TcpClient client = _listener.AcceptTcpClient();
                _server.AddConnection(client);
            }
        }

        ~Listener()
        {
            _listener.Stop();
        }
    }
}
