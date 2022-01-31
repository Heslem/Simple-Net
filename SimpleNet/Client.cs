using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleNet
{
    /// <summary>
    /// Class for connecting to server. The server uses this class to process clients
    /// </summary>
    public sealed class Client
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        private readonly Thread _recieveThread;

        public bool Connect { get; set; }

        public Action<Packet> Recieve { get; set; }

        public string ID { get; private set; }

        public Client(string ip = "127.0.0.1", int port = 8008) : this(new TcpClient(ip, port)) { }

        public Client(TcpClient client)
        {
            _client = client ?? throw new ArgumentNullException();
            _stream = client.GetStream();

            Connect = true;

            _recieveThread = new Thread(new ThreadStart(RecieveMessage));
            _recieveThread.Start();
        }

        private void RecieveMessage()
        {
            while (Connect)
            {
                if (_stream.DataAvailable)
                {
                    Packet packet = ReadPacket();
                    Recieve?.Invoke(packet);
                }
            }
        }

        #region Overloading metods (read)
        public int ReadInt()
        {
            byte[] buffer = new byte[4];
            _stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public float ReadFloat()
        {
            byte[] buffer = new byte[4];
            _stream.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        public ushort ReadUshort()
        {
            byte[] buffer = new byte[2];
            _stream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public short ReadShort()
        {
            byte[] buffer = new byte[2];
            _stream.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        public string ReadString(ushort couthBytes)
        {
            byte[] buffer = new byte[couthBytes];
            _stream.Read(buffer, 0, couthBytes);
            return Encoding.UTF8.GetString(buffer);
        }

        public Packet ReadPacket()
        {
            ushort lengthName = ReadUshort();
            string name = ReadString(lengthName);

            ushort lengthValue = ReadUshort();
            string value = ReadString(lengthValue);

            return new Packet(name, value);
        }
        #endregion

        #region Overloading metods (send)
        public void Send(int value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Send(short value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void Send(ushort value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public void Send(float value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Send(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            _stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Send packet to client.
        /// </summary>
        /// <param name="packet">A packet being sent</param>
        public void Send(Packet packet)
        {
            ushort lengthName = (ushort)Encoding.UTF8.GetByteCount(packet.Name);
            ushort lengthValue = (ushort)Encoding.UTF8.GetByteCount(packet.Value);

            Send(lengthName);
            Send(packet.Name);
            Send(lengthValue);
            Send(packet.Value);
        }
        #endregion

        ~Client()
        {
            _stream.Close();
            _client.Close();
        }
    }
}
