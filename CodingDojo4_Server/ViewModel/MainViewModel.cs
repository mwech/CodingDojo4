using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System;

namespace CodingDojo4_Server.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Server server;
        //defining port to be connected, not changing
        private const int port = 10100;
        //defining ip to be connected to, not changing
        private const string ip = "127.0.0.1";
        //initially not connected
        private bool isConnected = false;
        //Defining RelayCommands for all the three buttons in the gui
        public RelayCommand StartBtnClickCmd { get; set; }
        public RelayCommand StopBtnClickCmd { get; set; }
        public RelayCommand DropClientBtnClickCmd { get; set; }
        //ObservableCollection in which the names of the connected clients are saved
        //for the listbox that displays all the clients
        public ObservableCollection<string> Users { get; set; }
        //ObservableCollection in which the messages of the connected clients are saved
        //for the listbox that displays all the client messages
        public ObservableCollection<string> Messages { get; set; }
        //the name of the current selected user
        public string SelectedUser { get; set; }
        //property that saves the amount of chat messages; cant be set only get
        public int NoOfReceivedMessages
        {
            get
            {
                //length of entries in Observable collection
                return Messages.Count;
            }
        }
        public MainViewModel()
        {
            //Initiating the two ObservableCollections
            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<string>();

            //set command for start button
            StartBtnClickCmd = new RelayCommand(
                //first what is done when the start button is clicked
                () =>
                {
                    //new Server instance created
                    server = new Server(ip, port, UpdateGuiWithNewMessage);
                    //server starts to accept clients
                    server.StartAccepting();
                    //setting the isConnected checker
                    isConnected = true;
                },
                //secondly when is button enabled; only when server not started
                () => { return !isConnected; });

            //set command for stop button
            StopBtnClickCmd = new RelayCommand(
                //first what is done when the stop button is clicked
                () =>
                {
                    //disconnect all the clients, aborting
                    server.StopAccepting();
                    //updating the isConnected checker
                    isConnected = false;
                },
                //secondly when is button enabled; only when server is started
                () => { return isConnected; });

            //init Command for Drop button with CanExecute statement
            DropClientBtnClickCmd = new RelayCommand(
                //first what is done when the drop user button is clicked    
                () =>
                {
                    //the selected client in the listbox is disconnected by his name
                    server.DisconnectSpecificClient(SelectedUser);
                    // remove from GUI listbox
                    Users.Remove(SelectedUser); 
                },
                //secondly when is drop user button enabled; only if there are clients connected
                () => { return (SelectedUser != null); });
        }

        private void UpdateGuiWithNewMessage(string message)
        {
            //switch thread to GUI thread to write to GUI
            App.Current.Dispatcher.Invoke(() =>
            {
                //get the left part of the message; all the text before : is the users name
                string name = message.Split(':')[0];
                //check if user not in list
                if (!Users.Contains(name))
                {
                    //not in list => add it
                    Users.Add(name);
                }
                //write message
                Messages.Add(message);
                //do this to inform the GUI about the update of the received message counter!
                RaisePropertyChanged("NoOfReceivedMessages");
            });
        }
    }
}