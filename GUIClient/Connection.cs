using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GUIClient
{
    class Connection
    {
        public TcpClient client;
        private StreamWriter sw;
        private MainWindow window;
        public string log = "";
        Thread tReceiver;

        public Connection(MainWindow window, string IP, int port)
        {
            this.window = window;

            // Attempt to connect to server within 1 second
            // If not then cancel the connection
            client = new TcpClient();
            IAsyncResult result = client.BeginConnect(IP, port, null, null);
            if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1000)))
            {
                Close();
                throw new SocketException();
            }
           
            sw = new StreamWriter(client.GetStream());

            tReceiver = new Thread(() => MessageReceiver());
            tReceiver.Start();
        }

        private void MessageReceiver()
        {
            StreamReader sr = new StreamReader(client.GetStream());
            string message = "";

            while (true)
            {
                message = sr.ReadLine();
                // Basically makes this action run on the UI thread since this method runs on a different one
                window.Dispatcher.Invoke(new Action(() => { window.AddMessage(message); }));
            }
        }

        public void MessageSend(string message)
        {
            sw.WriteLine(message);
            sw.Flush();
        }

        public void Close()
        {
            MessageSend("/quit");
            tReceiver.Abort();
            client.Close();
        }
    }
}
