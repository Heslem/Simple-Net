using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleNet
{
    public sealed class Listener
    {
        private readonly TcpListener _listener;
        private readonly Thread _listenThread;

        private bool _working = false;

        public Listener(string ip = "127.0.0.1", int port = 8008)
        {
            _listener = new TcpListener(IPAddress.Parse(ip), port);
            _listenThread = new Thread(new ThreadStart(Listen));
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
                Server.GetServer().AddConnection(client);
            }
        }

        ~Listener()
        {
            _listener.Stop();
        }
    }
}
