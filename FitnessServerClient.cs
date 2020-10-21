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
        public bool isOnline
        {
            get; set;
        }

        public string UserName
        {
            get; set;
        }

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private byte[] _buffer = new byte[1024];
        private string _totalBuffer = "";


        public FitnessServerClient(TcpClient tcpClient)
        {
            this._tcpClient = tcpClient;
            isOnline = true;
            _stream = this._tcpClient.GetStream();
            _stream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(OnRead), null);
        }




        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = _stream.EndRead(ar);
                string receivedText = System.Text.Encoding.ASCII.GetString(_buffer, 0, receivedBytes);
                for (int i = 0; i < receivedBytes; i++)
                {
                    Console.WriteLine(receivedText[i]);
                }

                byte[] PartialBuffer = _buffer.Take(receivedBytes).ToArray();
                string Decrypted = Crypting.DecryptStringFromBytes(PartialBuffer);
                _totalBuffer += Decrypted;
            }
            catch (IOException)
            {
                //FitnessServer.Disconnect(this);
                return;
            }

            while (_totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = _totalBuffer.Substring(0, _totalBuffer.IndexOf("\r\n\r\n"));
                _totalBuffer = _totalBuffer.Substring(_totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                ProcessData(packetData);
            }

            _stream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(OnRead), null);
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
                    string loginUsername = packetData[1];
                    string loginPassword = packetData[2];

                    // Read the file and display it line by line.  
                    System.IO.StreamReader file = new System.IO.StreamReader(@"../../../LoginDB.txt");
                    bool match = false;
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] words = line.Split("SPLIT");
                        string tempLogin = words[0];
                        string tempPassword = words[1];

                        Console.WriteLine("Query: Does " + tempLogin + "match with " +
                                          Crypting.EncryptStringToString(loginUsername));
                        Console.WriteLine("Query: Does " + tempPassword + "match with " +
                                          Crypting.EncryptStringToString(loginPassword));
                        if (tempLogin.Equals((Crypting.EncryptStringToString(loginUsername))) &&
                            tempPassword.Equals(Crypting.EncryptStringToString(loginPassword)))
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
                        string registerUsername = packetData[1];
                        string registerPassword = packetData[2];
                        string otherData = packetData[3];

                        Console.WriteLine("We received this string... " + otherData);

                        //Encrypting
                        string encryptedUsernameString = Crypting.EncryptStringToString(registerUsername);
                        string encryptedPasswordString = Crypting.EncryptStringToString(registerPassword);
                        Console.WriteLine("Received a register packet! Writing this register to our .txt DB file ");
                        StreamWriter sw = new StreamWriter("../../../LoginDB.txt", true);
                        sw.WriteLine(encryptedUsernameString + "SPLIT" + encryptedPasswordString);

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

                        Database.WriteUserToDatabase(registerUsername, age, cm, kg, gender);

                        Write("FitnessClientRegister\r\nok\r\nok");
                        sw.Close();
                    }
                    catch (Exception e)
                    {
                        Write("FitnessClientRegister\r\nerror\r\nexception caught" + e.Message);
                        throw;
                    }

                    break;


            }
        }

        private bool AssertPacketData(string[] packetData, int requiredLength)
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

            var dataStringEncrypted = Crypting.EncryptStringToBytes(data + "\r\n\r\n");

            Debug.WriteLine("Non encrypted.. " + Encoding.ASCII.GetString(dataAsBytes));

            Debug.WriteLine("Encrypted " + Encoding.ASCII.GetString(dataStringEncrypted));

            _stream.Write(dataStringEncrypted, 0, dataStringEncrypted.Length);

            _stream.Flush();
        }
    }
}