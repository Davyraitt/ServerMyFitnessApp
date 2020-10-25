using ClientMyFitnessApp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ServerMyFitnessApp
{
    public static class Database
    {
        /* This method writes everything we need to the database.
        *
        * */

        //Connectionstring contains all the information the client needs to connect to the SQL server
        private static string connectionString = "Data Source=SQLServer;" +
                                                  "Initial Catalog=FitnessDatabase; Integrated Security=False;" +
                                                  "User id=sa;" +
                                                  "Password=Avans2020;";

        public static bool WriteFoodToDatabase(List<FoodItem> completeFoodList)
        {
            Console.WriteLine("WRITING TO DATABASE");


            //clearing the database for testing purposes
            using (var connection2 = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("DELETE FROM dbo.food", connection2))
                {
                    //opening the db connection
                    connection2.Open();
                    var result2 = command.ExecuteNonQuery();

                    // Check Error
                    if (result2 < 0)
                        return false;
                    Console.WriteLine("Error inserting data into Database!");
                }
            }

            //This string contains our Query we are going to send to the database

            var QueryString =
                "INSERT INTO dbo.Food (Name,Category,Kcal,Fat,Fiber,Contents) VALUES (@name,@category,@kcal,@fat,@fiber,@contents)";


            //For each item in the foodlist we set up an connection and add the food and its details.
            for (var i = 0; i < completeFoodList.Count; i++)
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand(QueryString, connection))
                    {
                        //Adding parameters we should add in the database, gets the info from foodlist
                        command.Parameters.AddWithValue("@name", completeFoodList[i].Name);
                        command.Parameters.AddWithValue("@category", completeFoodList[i].Category);
                        command.Parameters.AddWithValue("@kcal", completeFoodList[i].Kcal);
                        command.Parameters.AddWithValue("@fat", completeFoodList[i].Fat);
                        command.Parameters.AddWithValue("@fiber", completeFoodList[i].Fibers);

                        //If the foodcontent is empty, which happens sometimes. We write No Food Content Available
                        if (completeFoodList[i].FoodContents == null)
                            command.Parameters.AddWithValue("@contents", "No Food Content Available");

                        //If it is not empty we add the contents
                        else
                            command.Parameters.AddWithValue("@contents", completeFoodList[i].FoodContents);

                        //Execute the query
                        connection.Open();
                        var result = command.ExecuteNonQuery();

                        // Check manually for Errors
                        Console.WriteLine("Done writing item " + completeFoodList[i].Name + " to the database!");
                    }
                }

            return true;
        }

        public static bool WriteUserToDatabase(string name, int age, int length, double weight, string gender)
        {
            //converting length
            var lengthInMtr = length * 0.01;

            //calculating bmi
            var bmi = weight / (lengthInMtr * lengthInMtr);

            double bmr = 1700; //standard, we will change this later

            //This string contains our Query we are going to send to the database
            var QueryString =
                "INSERT INTO dbo.UserInfo (Name,Age,Length,Weight,Gender,BMI,BMR) VALUES (@name,@age,@length,@weight,@gender,@bmi,@bmr)";

            //calculating the base metabolic rate
            if (gender.Equals("Male")) bmr = 66.473 + (13.8 * weight) + (5.0033 * length) - (6.7550 * age);

            if (gender.Equals("Female")) bmr = 655.0955 + (9.5634 * weight) + (1.8496 * length) - (4.6756 * age);

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(QueryString, connection))
                {
                    //Adding parameters we should add in the database, gets the info from user
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@age", age);
                    command.Parameters.AddWithValue("@length", length);
                    command.Parameters.AddWithValue("@weight", weight);
                    command.Parameters.AddWithValue("@gender", gender);
                    command.Parameters.AddWithValue("@bmi", bmi);
                    command.Parameters.AddWithValue("@bmr", bmr);


                    //executign command
                    connection.Open();
                    var result = command.ExecuteNonQuery();

                    // Check Error
                    Console.WriteLine("Done writing item to the database!");
                }
            }


            return true;
        }

        public static void ClearDatabaseOfUsers()
        {
            //Method we can call if we want to clear the database, not called yet
            using (var connection2 = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("DELETE FROM dbo.UserInfo", connection2))
                {
                    connection2.Open();
                    var result2 = command.ExecuteNonQuery();
                }
            }
        }
    }
}