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

        //Variables
        private static TcpListener listener;

        private static List<FitnessServerClient> clients = new List<FitnessServerClient>();

        private static List<FoodItem> foodList;

        private static ArrayList foodKeyWords;

        private static List<FoodItem> completeFoodList;


        static void Main(string[] args)
        {
            

            Console.WriteLine("Starting server and waiting for clients.. on port 15243");

            //Uncomment this if you want to clear the databases
            //Database.ClearDatabaseOfUsers();

            //Uncomment this to fill the database with fooditems
            //This thread runs on the background and fills our database with food
            new Thread(() =>
             {
                 Thread.CurrentThread.IsBackground = true;
                 //Retreiving some fooditems
                 GetFoodWithAPI();
             }).Start();

            //Starting the server and listeners
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
            //Getting the food from the API

            //Gets everything ready we want to pass trough to the api
            APIInit();
            foodList = new List<FoodItem>();

            //For each keyword we call the URL API and get some food details
            for (int i = 0; i < foodKeyWords.Count; i++)
            {
                //Get the food from the API with the keyword
                foodList = FoodApi.RetrieveFromFoodApi((string)foodKeyWords[i]);

                //If we get a lot of fooditems (more then 3) we cap it at 3
                if (foodList.Count > 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        completeFoodList.Add(foodList[j]);
                    }
                }

                //If we dont get a lot we just get them all
                else
                {
                    for (int j = 0; j < foodList.Count; j++)
                    {
                        completeFoodList.Add(foodList[j]);
                    }
                }
            }

            //Prints the food items we got just to check
            for (int i = 0; i < completeFoodList.Count; i++)
            {
                Console.WriteLine("Food Item " + i);
                Console.WriteLine(completeFoodList[i].ToString());
                Console.WriteLine(" ---------- ");
            }

            //Writes the food to the database!
            Database.WriteFoodToDatabase(completeFoodList);
        }


        private static void APIInit()
        {
            foodKeyWords = new ArrayList();
            completeFoodList = new List<FoodItem>();

            //Adding food we want to get from the API and then enter in the database
            foodKeyWords.Add("Pizza");
            foodKeyWords.Add("Cabbage");
            foodKeyWords.Add("Protein");
            foodKeyWords.Add("Milk");
            foodKeyWords.Add("Yoghurt");
            foodKeyWords.Add("Shake");
            foodKeyWords.Add("Cheese");
            foodKeyWords.Add("Cake");
            foodKeyWords.Add("Spinach");
            foodKeyWords.Add("Lettuce");
            foodKeyWords.Add("Chocolate");
            foodKeyWords.Add("Champagne");
            foodKeyWords.Add("Coffee");
            foodKeyWords.Add("Apple");
            foodKeyWords.Add("Water");
            foodKeyWords.Add("Fanta");
            foodKeyWords.Add("Coke");
        }


        private static void OnConnect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);

            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");

            //Adds our client to the list of clients
            clients.Add(new FitnessServerClient(tcpClient));

            listener.BeginAcceptTcpClient(new AsyncCallback(OnConnect), null);
            //When the async method finish the processing,
            //AsyncCallback method is automatically called,
            //where post processing statements can be executed.
            //With this technique there is no need to poll or wait for the async thread to complete
        }

    }
}