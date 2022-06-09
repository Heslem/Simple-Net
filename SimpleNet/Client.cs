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

        public Guid ID { get; private set; }

        public Client(string ip = "127.0.0.1", int port = 8008) : this(new TcpClient(ip, port)) { }

        public Client(TcpClient client)
        {
            _client = client ?? throw new ArgumentNullException("Variable client was null.");
            _stream = client.GetStream();

            ID = Guid.NewGuid();
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

        private byte[] ReadDataFromStream(ushort bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            _stream.Read(buffer, 0, bufferSize);
            return buffer;
        }

        #region Overloading metods (read)
        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadDataFromStream(sizeof(int)), 0);
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadDataFromStream(sizeof(float)), 0);
        }

        public ushort ReadUshort()
        {
            return BitConverter.ToUInt16(ReadDataFromStream(sizeof(ushort)), 0);
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadDataFromStream(sizeof(short)), 0);
        }

        public string ReadString(in ushort couthBytes)
        {
            return Encoding.UTF8.GetString(ReadDataFromStream(couthBytes));
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
        public void Send(in int value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, sizeof(int));
        }

        public void Send(in short value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, sizeof(short));
        }

        public void Send(in ushort value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, sizeof(ushort));
        }

        public void Send(in float value)
        {
            _stream.Write(BitConverter.GetBytes(value), 0, sizeof(float));
        }

        public void Send(in string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            _stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Send packet to client.
        /// </summary>
        /// <param name="packet">A packet being sent</param>
        public void Send(in Packet packet)
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
