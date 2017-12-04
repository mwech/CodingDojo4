using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CodingDojo4.ViewModel
{

    public class MainViewModel : ViewModelBase
    {

        private Client clientcom;
        private bool isConnected = false;
        //Propperties will be used in the GUI
        public string ChatName { get; set; }
        public string Message { get; set; }
        //All received messages will be saved in an ObservableCollection, not a List
        public ObservableCollection<string> ReceivedMessages { get; set; }
        //Defining the properties for the buttons -> RelayCommands for control
        public RelayCommand ConnectBtnClickCmd { get; set; }
        public RelayCommand SendBtnClickCmd { get; set; }

        public MainViewModel()
        {
            Message = "";
            ReceivedMessages = new ObservableCollection<string>();
            ConnectBtnClickCmd = new RelayCommand(
                //first what is done when the button is clicked ...
                //creating new Client instance
                () =>
                {
                    //Client(string ip, int port, Action<string> messageInformer, Action abortInformer)
                    isConnected = true;
                    clientcom = new Client("127.0.0.1", 10100, new Action<string>(NewMessageReceived), ClientDisconnected);

                },
                //secondly define if button is clickable or not 
            () =>
            {
                return (!isConnected);
            });

            SendBtnClickCmd = new RelayCommand(
                //what happens if button is clicked ..
                //message with username and the message content is sent
                () => {
                    clientcom.Send(ChatName + ": " + Message);
                    //New entry for the sent message in the ListBox
                    ReceivedMessages.Add("YOU: " + Message);                    
                },
                //secondly define if button is clickable or not 
                () => { return (isConnected && Message.Length >= 1); });
        }

        private void ClientDisconnected()
        {
            isConnected = false;
            //do this to force the update of the button visibility otherwise change is done after focus change (clicking into gui)
            CommandManager.InvalidateRequerySuggested();
        }

        private void NewMessageReceived(string message)
        {
            //write new message in Collection to display in GUI
            //switch thread to GUI thread to avoid problems
            App.Current.Dispatcher.Invoke(() =>
            {
                ReceivedMessages.Add(message);
            });
        }
    }
}