using SharedClass;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ServerMyFitnessApp
{

    class FitnessServerClient
    {

        //Variables
        public bool isOnline
        {
            get; set;
        }

        public string UserName
        {
            get; set;
        }

        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer = "";


        public FitnessServerClient(TcpClient tcpClient)
        {
            //constructor setting variables we are online etc..
            this.tcpClient = tcpClient;
            isOnline = true;

            //Getting the stream from the tcpclient
            stream = this.tcpClient.GetStream();
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(Read), null);
            //When the async method finish the processing,
            //AsyncCallback method is automatically called,
            //where post processing statements can be executed.
            //With this technique there is no need to poll or wait for the async thread to complete
        }




        private void Read(IAsyncResult ar)
        {
            try
            {
                //gets the bytes we received from the ar
                int receivedBytes = stream.EndRead(ar);

                //converts the bytes to a string
                string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);

                //prints the text we received
                for (int i = 0; i < receivedBytes; i++)
                {
                    Console.WriteLine(receivedText[i]);
                }

                //Takes the receivedbytes and makes an bytearray
                byte[] PartialBuffer = buffer.Take(receivedBytes).ToArray();

                //Decrypt the received bytearray
                string Decrypted = Crypting.DecryptStringFromBytes(PartialBuffer);

                //Adds the decrypted string to the totalbuffer
                totalBuffer += Decrypted;
            }
            catch (IOException)
            {
                return;
            }

            //Now we are going to read.. as long as the buffer contains 2 newlines
            while (totalBuffer.Contains("\r\n\r\n"))
            {
                //Gets the first part of the TotalBuffer packet
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);

                //Adds the packetdata to the stringarray
                string[] packetData = Regex.Split(packet, "\r\n");

                //Calls the method processdata 
                ProcessData(packetData);
            }

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(Read), null);
        }

        private void ProcessData(string[] packetData)
        {
            // Console.WriteLine($"Got a packet: {packetData[0]}");


            switch (packetData[0])
            {
                //If we receive a Login packet:
                case "FitnessClientLogin":
                    //setting up temporary variables
                    string line;
                    int counter = 0;
                    bool match = false;
                    string loginUsername = packetData[1];
                    string loginPassword = packetData[2];
                    System.IO.StreamReader file = new System.IO.StreamReader(@"../../../LoginDB.txt");


                    // Read the file line by line, keeps reading till the last line
                    while ((line = file.ReadLine()) != null)
                    {
                        //We split the username and password by using ;, so we split it here
                        string[] words = line.Split(";");
                        string tempLogin = words[0];
                        string tempPassword = words[1];

                        Console.WriteLine("Query: Does " + tempLogin + "match with " +
                                          Crypting.EncryptStringToString(loginUsername));
                        Console.WriteLine("Query: Does " + tempPassword + "match with " +
                                          Crypting.EncryptStringToString(loginPassword));

                        //If the username and password entered matches with the one in our .txt database...
                        if (tempLogin.Equals((Crypting.EncryptStringToString(loginUsername))) &&
                            tempPassword.Equals(Crypting.EncryptStringToString(loginPassword)))
                        {
                            match = true;
                        }


                        counter++;
                    }

                    if (match)
                    {
                        //If we have a match we write back the password is OK
                        Console.WriteLine("Writing back: Password is OK");
                        Write("FitnessClientLogin\r\nok");
                    }
                    else
                    {
                        //Writing back: Password is wrong, we dont have a match
                        Console.WriteLine("Writing back: Password is wrong");
                        Write("FitnessClientLogin\r\nerror\r\nIncorrect password");
                    }

                    //Close the file
                    file.Close();


                    break;


                //If we receive a Register packet:
                case "FitnessClientRegister":

                    try
                    {
                        //Gets the received data
                        string registerUsername = packetData[1];
                        string registerPassword = packetData[2];
                        string otherData = packetData[3];

                        //Encrypting
                        string encryptedUsernameString = Crypting.EncryptStringToString(registerUsername);
                        string encryptedPasswordString = Crypting.EncryptStringToString(registerPassword);
                        Console.WriteLine("Received a register packet! Writing this register to our .txt DB file ");
                        StreamWriter sw = new StreamWriter("../../../LoginDB.txt", true);
                        sw.WriteLine(encryptedUsernameString + ";" + encryptedPasswordString);

                        //Splitting the rest of the data 
                        Console.WriteLine(otherData);
                        string[] words = otherData.Split(';');

                        int age = Int32.Parse(words[0]);
                        int cm = Int32.Parse(words[1]);
                        int kg = Int32.Parse(words[2]);
                        string gender = words[3];

                        // foreach (var word in words)
                        // {
                        //     System.Console.WriteLine($"<{word}>");
                        //
                        // }

                        //Writing the user to the database also
                        Database.WriteUserToDatabase(registerUsername, age, cm, kg, gender);


                        //Writing to the registerclient we succesfully did everything.
                        Write("FitnessClientRegister\r\nok\r\nok");
                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        //Writing to the registerclient we failed registering.
                        Write("FitnessClientRegister\r\nerror\r\nexception caught" + e.Message);
                        throw;
                    }

                    break;


            }
        }


        public void Write(string data)
        {
            //key and initialization vector (IV).

            //Adds  the data we want to write to an array
            byte[] dataAsBytes = System.Text.Encoding.ASCII.GetBytes(data + "\r\n\r\n");

            //Encrypting the array..
            byte[] dataArrayEncrypted = Crypting.EncryptStringToBytes(data + "\r\n\r\n");

            //Checking
            Debug.WriteLine("Non encrypted.. " + Encoding.ASCII.GetString(dataAsBytes));

            Debug.WriteLine("Encrypted " + Encoding.ASCII.GetString(dataArrayEncrypted));

            //Wrie the encrypted byte[]
            stream.Write(dataArrayEncrypted, 0, dataArrayEncrypted.Length);

            //Empty the stream
            stream.Flush();
        }
    }
}