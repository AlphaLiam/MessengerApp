using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUIClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string IP;
        int port;
        Connection connection;
        string UserName;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {
            // don't add messages to log until the user has chosen a username
            if (UserName != null)
            {
                Log.Text += message + "\n";
                Log.ScrollToEnd();
            }
        }

        private void EnterMessage_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (e.Key == Key.Enter && tb.Text.Length > 0 && connection != null)
            {
                connection.MessageSend(tb.Text);
                tb.Text = "";
            }
        }

        // Occurs when user closes program
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Have to close connection or else this class will continue to run even if the window closes
            connection.Close();
            connection = null;
            Application.Current.Shutdown();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If user already connected to a server, diconnect them before connecting them to another
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
                connection = new Connection(this, IP, port);
                ConnectionStatus.Content = "Connected";
                ConnectionStatus.Foreground = new SolidColorBrush(Colors.LimeGreen);
                connection.MessageSend("/name " + UserName);
            } catch (Exception exception)
            {
                connection = null;
                ConnectionStatus.Content = "Not Connected";
                ConnectionStatus.Foreground = new SolidColorBrush(Colors.Firebrick);
            }
            Log.Text = "";
        }

        private void IPBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            IP = tb.Text;
        }

        private void PortBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            port = Int32.Parse(tb.Text);
        }

        private void UserNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            UserName = tb.Text;
        }
    }
}
