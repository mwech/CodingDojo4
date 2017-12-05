using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CodingDojo4_Server
{
    internal class ClientHandler
    {
        //Name of the client
        public string Name { get; private set; }
        //delegate
        private Action<string, Socket> action;
        //Another client buffer for receiving messages
        private byte[] buffer = new byte[512];
        //never changing end message
        const string endMessage = "@quit";
        //a thread for that listens for messages from others
        private Thread clientReceiveThread;
        //socket for the client
        public Socket Clientsocket { get; private set; }

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            //initiating attribute with parameter values 
            //for socket
            this.Clientsocket = socket;
            //for delegate
            this.action = action;
            //start message receiving in a new thread
            //call method receive
            clientReceiveThread = new Thread(Receive);
            //thread is now started
            clientReceiveThread.Start();
        }
        private void Receive()
        {
            string message = "";
            //as long as the client receives no message that he must be disconnected from server
            while (!message.Equals(endMessage))
            {
                //determine the length of the received byte message
                int length = Clientsocket.Receive(buffer);
                //converting message from bytes to string
                message = Encoding.UTF8.GetString(buffer, 0, length);
                //set name property if not already done
                if (Name == null && message.Contains(":"))
                {
                    //Taking the left part; part before :
                    Name = message.Split(':')[0];
                }
                //inform GUI via delegate
                action(message, Clientsocket);
            }
            //as the connection from the client to the server is closed
            //no more message receiving
            Close();
        }

        public void Send(string message)
        {
            //sending message retrieved from parameter to server
            Clientsocket.Send(Encoding.UTF8.GetBytes(message));
        }

        public void Close()
        {
            //sends endmessage to client 
            Send(endMessage);
            //disconnects 
            Clientsocket.Close(1);
            //abort client threads
            clientReceiveThread.Abort();
        }
    }
}