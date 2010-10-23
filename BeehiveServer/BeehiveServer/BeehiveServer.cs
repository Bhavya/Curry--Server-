using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Application
{

    class Program
    {
        //global variables
        static Int32 portNo;
        static System.Net.IPAddress localAdd;
        static TcpListener listener;

        static void header()
        {
            Console.WriteLine("===============================================================================");
            Console.WriteLine("                          Beehive Server v 1.0.0.0                             ");
            Console.WriteLine("                             by Bhavya Kashyap                                 ");
            Console.WriteLine("=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
        }

        static void intro()
        {
            Console.WriteLine("Welcome to Beehive Server. Please select an option below:\n");
            settings();
        }

        static void settings()
        {
            Console.WriteLine("d - connect with default settings on port 500.");
            Console.WriteLine("n - connect with new settings");

            string option = Console.ReadLine();
            if (option.Equals("n"))
            {
                Console.WriteLine("Port Number:");
                string internalPort = Console.ReadLine();

                try
                {
                    portNo = Int32.Parse(internalPort);
                }
                catch
                {
                    Console.WriteLine("Invalid port number provided.");
                    return;
                }
            }
            else if (option.Equals("d"))
            {
                try
                {
                    portNo = Int32.Parse("500");
                }
                catch
                {
                    Console.WriteLine("Invalid port number provided.");
                    return;
                }
            }

        }

        static Int32 startListener()
        {
            string internalIP = "127.0.0.1";
            try
            {
                localAdd = System.Net.IPAddress.Parse(internalIP);
            }
            catch
            {
                Console.WriteLine("No localhost IP set.");
                return 0;
            }

            listener = new TcpListener(localAdd, portNo);
            try
            {
                listener.Start();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("TCP Listener failed to start." + e);
                return 0;
            }
        }

        static void acceptTCPRequest()
        {
            while (true)
            {
                ChatClient user = new ChatClient(listener.AcceptTcpClient());
            }

        }

        static void Main(string[] args)
        {
            header();
            intro();
            if (startListener() == 1)
            {
                Console.WriteLine("Setting up listener at " + localAdd + " on port " + portNo);
                Console.WriteLine("Waiting for connections...");
                acceptTCPRequest();
            }
            Console.ReadLine();
        }
    }
}