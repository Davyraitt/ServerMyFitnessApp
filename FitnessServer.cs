using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClientMyFitnessApp;

namespace ServerMyFitnessApp
{
    class FitnessServer
    {
        private static TcpListener listener;

        private static List<FitnessServerClient> clients = new List<FitnessServerClient>();

        private static List<FoodItem> FoodList;

        private static ArrayList FoodKeyWords;

        private static List<FoodItem> CompleteFoodList;


        static void Main(string[] args)
        {
            //Database.ClearDatabaseOfUsers();

            Console.WriteLine("Starting server and waiting for clients.. on port 15243");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                //Retreiving some fooditems
                GetFoodWithAPI();
            }).Start();

            listener = new TcpListener(IPAddress.Any, 15243);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.ReadLine();


            while (true)
            {
            }
        }

        private static void GetFoodWithAPI()
        {
            APIInit();
            FoodList = new List<FoodItem>();

            for (int i = 0; i < FoodKeyWords.Count; i++)
            {
                FoodList = FoodAPI.RetrieveFromFoodAPI((string) FoodKeyWords[i]);

                if (FoodList.Count > 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        CompleteFoodList.Add(FoodList[j]);
                    }
                }

                else
                {
                    for (int j = 0; j < FoodList.Count; j++)
                    {
                        CompleteFoodList.Add(FoodList[j]);
                    }
                }
            }


            for (int i = 0; i < CompleteFoodList.Count; i++)
            {
                Console.WriteLine("Food Item " + i);
                Console.WriteLine(CompleteFoodList[i].ToString());
                Console.WriteLine(" ---------- ");
            }

            Database.WriteFoodToDatabase(CompleteFoodList);
        }


        private static void APIInit()
        {
            FoodKeyWords = new ArrayList();
            CompleteFoodList = new List<FoodItem>();

            //Adding food we want to get from the API and then enter in the database

            FoodKeyWords.Add("Pizza");
            FoodKeyWords.Add("Cabbage");
            FoodKeyWords.Add("Protein");
            FoodKeyWords.Add("Milk");
            FoodKeyWords.Add("Yoghurt");
            FoodKeyWords.Add("Shake");
            FoodKeyWords.Add("Cheese");
            FoodKeyWords.Add("Cake");
            FoodKeyWords.Add("Spinach");
            FoodKeyWords.Add("Lettuce");
            FoodKeyWords.Add("Chocolate");
            FoodKeyWords.Add("Champagne");
            FoodKeyWords.Add("Coffee");
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