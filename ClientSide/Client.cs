using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class Client
{
    public static TcpClient client;
    private static StreamWriter sw;

    static void Main(string[] args)
    {
        Console.Write("Enter Username: ");
        string name = Console.ReadLine();

        Console.WriteLine("Connecting...");

        // This port was already open on my router
        client = new TcpClient("127.0.0.1", 25565);
        Console.WriteLine("Connected");

        Thread tReceiver = new Thread(() => MessageReceiver());
        tReceiver.Start();

        sw = new StreamWriter(client.GetStream());
        MessageSend("/name " + name);
        
        string message = "";

        // Continually allow user to send messages as long as they don't type the quit command
        while (!message.Equals("/quit"))
        {
            message = Console.ReadLine();
            MessageSend(message);
        }
        tReceiver.Abort();
    }

    // This runs conncurently with the main method due to multithreading
    // This method is simply for receiving messages from the server
    private static void MessageReceiver()
    {
        StreamReader sr = new StreamReader(client.GetStream());
        string message = "";

        while (true)
        {
            message = sr.ReadLine();
            Console.WriteLine(message);
        }

    }

    public static void MessageSend(string message)
    {
        sw.WriteLine(message);
        sw.Flush();
    }
}

