using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServerMyFitnessApp
{
    class FitnessServer
    {

        private static TcpListener listener;
       // private static List<FitnessServerClient> clients = new List<FitnessServerClient>();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server and waiting for clients..");

            listener = new TcpListener(IPAddress.Any, 15243);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);

            while (true)
            {

            }

        }

        private static void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            //check if the client already excists
            //clients.Add(new FitnessServerClient(tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        // internal static void Disconnect(FitnessServerClient client)
        // {
        //     clients.Remove(client);
        //     client.Disconnect();
        //     Console.WriteLine("Client disconnected");
        // }


    }
}
