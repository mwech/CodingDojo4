using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CodingDojo4
{
    public class Client
    {
        //byte buffer because the messages to be received are not string 
        byte[] buffer = new byte[512];
        Socket clientsocket;
        //Delegate to inform gui
        Action<string> MessageInformer;
        //Delegate to reseting the connection with the client
        Action AbortInformer;

        public Client(string ip, int port, Action<string> messageInformer, Action abortInformer)
        {
            //Trying to connect but server might not be ready
            try
            {
                //Initiating the delegates
                this.AbortInformer = abortInformer;
                this.MessageInformer = messageInformer;
                //Setting up new tcp client
                TcpClient client = new TcpClient();
                //Connecting with retrieved values for ip and port
                client.Connect(IPAddress.Parse(ip), port);
                //client.Client returns Socket type
                clientsocket = client.Client;
                //if everything has worked, the client can now receive messages 
                StartReceiving();
            }
            catch (Exception)
            {
                //Connection could not be provided, executing delegates
                messageInformer("Server not ready");
                AbortInformer();
            }
        }
        private void StartReceiving()
        {
            //The receiving method must be started in a new thread
            Task.Factory.StartNew(Receive);
        }
        private void Receive()
        {
            string message = "";
            //As long as server does not disconnect the client
            while (!message.Equals("@quit"))
            {
                //Getting the length of the message
                int length = clientsocket.Receive(buffer);
                //Converting message to string
                message = Encoding.UTF8.GetString(buffer, 0, length);
                //inform GUI via delegate
                MessageInformer(message);
            }
            //As finally a quit message was received, the client will now be disconnected
            Close();
        }
        private void Close()
        {
            //socket will be closed, shutting down the connection
            clientsocket.Close();
            //executing delegate to inform about event
            AbortInformer();
        }
        public void Send(string message)
        {
            //if client is connected
            if (clientsocket != null) 
            {
                //the message received from the parameter is sent
                clientsocket.Send(Encoding.UTF8.GetBytes(message));
            }
        }
    }
}
