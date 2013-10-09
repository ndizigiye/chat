using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WindowsFormsApplication2
{
    public class Client
    {
        private readonly TcpClient _clientSocket = new TcpClient();
        private NetworkStream serverStream;

        public void Connect()
        {
            _clientSocket.Connect("127.0.0.1", 9000);
            serverStream = _clientSocket.GetStream();
            Debug.WriteLine("Client connected!");
        }

        public string Send(string message)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            return message;
        }

        public void ReceiveData()
        {
            var t = new Thread(Receive);
            t.Start();
        }
        public void Receive()
        {
            while (serverStream.CanRead)
            {
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)_clientSocket.ReceiveBufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                Debug.WriteLine("From Server: "+ returndata);
            }
        }
    }
}
