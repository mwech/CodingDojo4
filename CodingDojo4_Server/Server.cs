using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodingDojo4_Server
{
    public class Server
    {
        Socket serverSocket;
        //all connected clients will be added to this List
        List<ClientHandler> clients = new List<ClientHandler>();
        //gui updating delegate
        Action<string> GuiUpdater;
        //handles accepting of new clients
        Thread acceptingThread;

        public Server(string ip, int port, Action<string> guiUpdater)
        {
            //Initiating attributes
            GuiUpdater = guiUpdater;
            //Creating a Socket for the server (remember also created socket for client)
            //setting it up with ipv4, tcp, stream describes the way of communication
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Connect socket with ip and port
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            //5 is some number, could be anything else
            serverSocket.Listen(5);
        }
        public void StartAccepting()
        {
            //The process of the server accepting clients is put into an own thread
            //The Accept method defines how the process of accepting a client works in detail
            acceptingThread = new Thread(new ThreadStart(Accept));
            //works behind the scenes
            acceptingThread.IsBackground = true;
            //thread now started
            acceptingThread.Start();
        }
        private void Accept()
        {
            //as long as the accepting thread is not stopped
            while (acceptingThread.IsAlive)
            {
                //Problems possible for instance if server is not active, use try catch
                try
                {
                    //in the List of clients new client will be added (creating a new client handler)
                    //serverSocket.Accept() returns a socket
                    clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(NewMessageReceived)));
                }
                //something did not work -> raising an Exception
                catch (Exception e)
                {
                //executed if serversocket.close is called -> problem, something did not work
                }
            }
        }
        public void StopAccepting()
        {
            //serverSocket is shut down
            serverSocket.Close();
            //thread which accepts clients when it is active also shut down, no more accepting clients
            acceptingThread.Abort(); //abort accepting thread
            //close all client threads and connections
            foreach (var item in clients)
            {
                //calling method close() from ClientHandler class; disconnecting clients
                item.Close();
            }
            //remove all entries from the list, no more clients 
            clients.Clear();
        }
        public void DisconnectSpecificClient(string name)
        {
            //going through all active clients in the list
            foreach (var item in clients)
            {
                //if the name of a client matches the parameter value 
                if (item.Name.Equals(name))
                {
                    //client will be disconnected, calling method Close from ClientHandler class
                    item.Close();
                    //remove Clienthandler object from list; 
                    clients.Remove(item);
                    //that works only if we break the foreach after that operation
                    break;
                }
            }
        }

        private void NewMessageReceived(string message, Socket senderSocket)
        {
            //update gui
            GuiUpdater(message);
            //going through all the clients in the List
            foreach (var item in clients)
            {
                //check that the sender does not receive his own message
                if (item.Clientsocket != senderSocket)
                {
                    //send the message to all other clients
                    //method is defined in ClientHandler class
                    item.Send(message);
                }
            }
        }
    }
}
