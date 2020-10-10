﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServerMyFitnessApp
{
    class FitnessServer
    {

        private static TcpListener listener;

        private static List<FitnessServerClient> clients = new List<FitnessServerClient>();

        private static ArrayList FoodList;

       // private static List<FitnessServerClient> clients = new List<FitnessServerClient>();

        static void Main(string[] args)
        {
            //Retreiving some fooditems
            // FoodList = new ArrayList();
            // FoodList = FoodAPI.RetrieveFromFoodAPI("Light");
            //
            // Console.WriteLine("Foodlist size.." + FoodList.Count);
            //
            // for (int i = 0; i < FoodList.Count; i++)
            // {
            //     Console.WriteLine("Food Item " + i);
            //     Console.WriteLine(FoodList[i].ToString());
            //     Console.WriteLine(" ---------- ");
            //    
            // }

            Console.WriteLine("Starting server and waiting for clients.. on port 15243");

            listener = new TcpListener(IPAddress.Any, 15243);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.ReadLine();

            while (true)
            {

            }

        }

        private static void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            //check if the client already excists
            clients.Add(new FitnessServerClient(tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        internal static void Broadcast(string packet)
        {
            foreach (var client in clients)
            {
                client.Write(packet);
            }
        }

        internal static void Disconnect(FitnessServerClient client)
        {
            clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }


    }
}
