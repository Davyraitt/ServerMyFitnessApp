using ClientMyFitnessApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerMyFitnessApp
{
    class FitnessServer
    {
        private static TcpListener _listener;

        private static List<FitnessServerClient> _clients = new List<FitnessServerClient>();

        private static List<FoodItem> _foodList;

        private static ArrayList _foodKeyWords;

        private static List<FoodItem> _completeFoodList;


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

            _listener = new TcpListener(IPAddress.Any, 15243);
            _listener.Start();
            _listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            Console.ReadLine();


            while (true)
            {
            }
        }

        private static void GetFoodWithAPI()
        {
            APIInit();
            _foodList = new List<FoodItem>();

            for (int i = 0; i < _foodKeyWords.Count; i++)
            {
                _foodList = FoodApi.RetrieveFromFoodApi((string)_foodKeyWords[i]);

                if (_foodList.Count > 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        _completeFoodList.Add(_foodList[j]);
                    }
                }

                else
                {
                    for (int j = 0; j < _foodList.Count; j++)
                    {
                        _completeFoodList.Add(_foodList[j]);
                    }
                }
            }


            for (int i = 0; i < _completeFoodList.Count; i++)
            {
                Console.WriteLine("Food Item " + i);
                Console.WriteLine(_completeFoodList[i].ToString());
                Console.WriteLine(" ---------- ");
            }

            Database.WriteFoodToDatabase(_completeFoodList);
        }


        private static void APIInit()
        {
            _foodKeyWords = new ArrayList();
            _completeFoodList = new List<FoodItem>();

            //Adding food we want to get from the API and then enter in the database

            _foodKeyWords.Add("Pizza");
            _foodKeyWords.Add("Cabbage");
            _foodKeyWords.Add("Protein");
            _foodKeyWords.Add("Milk");
            _foodKeyWords.Add("Yoghurt");
            _foodKeyWords.Add("Shake");
            _foodKeyWords.Add("Cheese");
            _foodKeyWords.Add("Cake");
            _foodKeyWords.Add("Spinach");
            _foodKeyWords.Add("Lettuce");
            _foodKeyWords.Add("Chocolate");
            _foodKeyWords.Add("Champagne");
            _foodKeyWords.Add("Coffee");
        }


        private static void OnConnect(IAsyncResult ar)
        {
            var tcpClient = _listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            //check if the client already excists
            _clients.Add(new FitnessServerClient(tcpClient));
            _listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
        }

        internal static void Broadcast(string packet)
        {
            foreach (var client in _clients)
            {
                client.Write(packet);
            }
        }

        internal static void Disconnect(FitnessServerClient client)
        {
            _clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }
    }
}