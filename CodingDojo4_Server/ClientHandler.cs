using System;
using System.Net.Sockets;

namespace CodingDojo4_Server
{
    internal class ClientHandler
    {
        private Socket socket;
        private Action<string, Socket> action;

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            this.socket = socket;
            this.action = action;
        }

        public Socket Clientsocket { get; internal set; }
        public object Name { get; internal set; }

        internal void Send(string message)
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}