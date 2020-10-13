using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ServerMyFitnessApp
{
    using ServerMyFitnessApp_Crypting = ServerMyFitnessApp.Crypting;

    class FitnessServerClient
    {
        public bool isOnline { get; set; }

        public string UserName { get; set; }

        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer = "";


        public FitnessServerClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.isOnline = true;
            this.stream = this.tcpClient.GetStream();

            

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

       


        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);

                string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);


                for (int i = 0; i < receivedBytes; i++)
                {
                    Console.WriteLine(receivedText[i]);
                }

                byte[] PartialBuffer = buffer.Take(receivedBytes).ToArray();

                String Decrypted = ServerMyFitnessApp_Crypting.DecryptStringFromBytes(PartialBuffer);

                totalBuffer += Decrypted;
            }
            catch (IOException)
            {
                //FitnessServer.Disconnect(this);
                return;
            }

            while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                ProcessData(packetData);
            }

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void ProcessData(string[] packetData)
        {
            Console.WriteLine($"Got a packet: {packetData[0]}");
            switch (packetData[0])
            {
                case "FitnessClientLogin":
                    //init stuff
                    string line;
                    int counter = 0;
                    string LoginUsername = packetData[1];
                    string LoginPassword = packetData[2];

                    // Read the file and display it line by line.  
                    System.IO.StreamReader file = new System.IO.StreamReader(@"../../../LoginDB.txt");
                    bool match = false;
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] words = line.Split("SPLIT");
                        string templogin = words[0];
                        string temppassword = words[1];

                        Console.WriteLine("Query: Does " + templogin + "match with " +
                                          Crypting.EncryptStringToString(LoginUsername));
                        Console.WriteLine("Query: Does " + temppassword + "match with " +
                                          Crypting.EncryptStringToString(LoginPassword));
                        if (templogin.Equals((Crypting.EncryptStringToString(LoginUsername))) &&
                            temppassword.Equals(Crypting.EncryptStringToString(LoginPassword)))
                        {
                            match = true;
                        }

                        counter++;
                    }

                    if (match)
                    {
                        Write("FitnessClientLogin\r\nok");
                    }
                    else
                    {
                        Write("FitnessClientLogin\r\nerror\r\nIncorrect password");
                    }

                    file.Close();


                    break;

                case "FitnessClientRegister":

                    try
                    {
                        string RegisterUsername = packetData[1];
                        string RegisterPassword = packetData[2];
                        string OtherData = packetData[3];

                        Console.WriteLine("We received this string... " + OtherData);

                        //Encrypting
                        string EncryptedUsernameString = Crypting.EncryptStringToString(RegisterUsername);
                        string EncryptedPasswordString = Crypting.EncryptStringToString(RegisterPassword);
                        Console.WriteLine("Received a register packet! Writing this register to our .txt DB file ");
                        StreamWriter sw = new StreamWriter("../../../LoginDB.txt", true);
                        sw.WriteLine(EncryptedUsernameString + "SPLIT" + EncryptedPasswordString);

                        //Splitting the rest of the data 
                        Console.WriteLine(OtherData);
                        string[] words = OtherData.Split(';');

                        int age = Int32.Parse(words[0]);
                        int cm = Int32.Parse(words[1]);
                        int kg = Int32.Parse(words[2]);
                        string gender = words[3];

                        // foreach (var word in words)
                        // {
                        //     System.Console.WriteLine($"<{word}>");
                        //
                        // }

                        Database.WriteUserToDatabase(RegisterUsername, age, cm, kg, gender);

                        Write("FitnessClientRegister\r\nok\r\nok");
                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        Write("FitnessClientRegister\r\nerror\r\nexception caught");
                        throw;
                    }

                    break;


            }
        }

        private bool assertPacketData(string[] packetData, int requiredLength)
        {
            if (packetData.Length < requiredLength)
            {
                Write("error");
                return false;
            }

            return true;
        }

        public void Write(string data)
        {
            var dataAsBytes = System.Text.Encoding.ASCII.GetBytes(data + "\r\n\r\n");

            var dataStringEncrypted = ServerMyFitnessApp_Crypting.EncryptStringToBytes(data + "\r\n\r\n");

            Debug.WriteLine("Non encrypted.. " + Encoding.ASCII.GetString(dataAsBytes));

            Debug.WriteLine("Encrypted " + Encoding.ASCII.GetString(dataStringEncrypted));

            stream.Write(dataStringEncrypted, 0, dataStringEncrypted.Length);

            stream.Flush();
        }
    }
}