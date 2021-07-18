using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


class Server
{
    static List<Thread> threads = new List<Thread>();
    static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        Console.WriteLine("Waiting for connection...");

        TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 25565);
        listener.Start();

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            StreamReader sr = new StreamReader(client.GetStream());

            Console.WriteLine("Client Connected");

            Thread t = null;
            threads.Add(t);
            clients.Add(client);
            t = new Thread(() => ClientHandle(client, t));
            t.Start();

        }
    }

    private static void ClientHandle(TcpClient client, Thread t)
    {
        string clientName = "User";
        StreamReader sr = new StreamReader(client.GetStream());
        StreamWriter sw = new StreamWriter(client.GetStream());

        string received = "";
        while (received != null)
        {
            received = sr.ReadLine();

            if (received.Length > 7 && received.Substring(0,6).Equals("/name "))
            {
                string oldName = clientName;
                clientName = received.Substring(6, received.Length - 6);
                Console.WriteLine(oldName + " changed their name to " + clientName);
            }
            else if (received.Equals("/quit"))
            {
                received = null;
            } else
            {
                Console.WriteLine(clientName + ": " + received);
                SendToClients(clientName + ": " + received);
            }

        }
        threads.Remove(t);
        clients.Remove(client);
        Console.WriteLine(clientName + " disconnected...");
        t.Abort();
    }

    private static void SendToClients(string message)
    {
        for (int i = 0; i < clients.Count; i++)
        {
            StreamWriter sw = new StreamWriter(clients.ElementAt(i).GetStream());
            sw.WriteLine(message);
            sw.Flush();
        }
    }
}

