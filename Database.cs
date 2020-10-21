using ClientMyFitnessApp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ServerMyFitnessApp
{
    public static class Database
    {
        private static string _connectionString = "Data Source=SQLServer;" +
                                                 "Initial Catalog=FitnessDatabase; Integrated Security=False;" +
                                                 "User id=sa;" +
                                                 "Password=Avans2020;";

        public static bool WriteFoodToDatabase(List<FoodItem> completeFoodList)
        {
            Console.WriteLine("WRITING TO DATABASE");


            //clearing the database for testing purposes
            using (var connection2 = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DELETE FROM dbo.food", connection2))
                {
                    connection2.Open();
                    var result2 = command.ExecuteNonQuery();

                    // Check Error
                    if (result2 < 0)
                        return false;
                    Console.WriteLine("Error inserting data into Database!");
                }
            }

            //refilling the database

            var QueryString =
                "INSERT INTO dbo.Food (Name,Category,Kcal,Fat,Fiber,Contents) VALUES (@name,@category,@kcal,@fat,@fiber,@contents)";

            for (var i = 0; i < completeFoodList.Count; i++)
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(QueryString, connection))
                    {
                        command.Parameters.AddWithValue("@name", completeFoodList[i].Name);
                        command.Parameters.AddWithValue("@category", completeFoodList[i].Category);
                        command.Parameters.AddWithValue("@kcal", completeFoodList[i].Kcal);
                        command.Parameters.AddWithValue("@fat", completeFoodList[i].Fat);
                        command.Parameters.AddWithValue("@fiber", completeFoodList[i].Fibers);

                        if (completeFoodList[i].FoodContents == null)
                            command.Parameters.AddWithValue("@contents", "No Food Content Avaiable");

                        else
                            command.Parameters.AddWithValue("@contents", completeFoodList[i].FoodContents);


                        connection.Open();
                        var result = command.ExecuteNonQuery();

                        // Check Error
                        Console.WriteLine("Done writing item " + completeFoodList[i].Name + " to the database!");
                    }
                }

            return true;
        }

        public static bool WriteUserToDatabase(string name, int age, int length, double weight, string gender)
        {
            Console.WriteLine("WRITING TO DATABASE");


            //deleting the database just for testing purposes
            //calculate the unknowns
            var lengthInMtr = length * 0.01;
            var bmi = weight / (lengthInMtr * lengthInMtr);

            double bmr = 1700;


            var QueryString =
                "INSERT INTO dbo.UserInfo (Name,Age,Length,Weight,Gender,BMI,BMR) VALUES (@name,@age,@length,@weight,@gender,@bmi,@bmr)";


            if (gender.Equals("Male")) bmr = 66 + weight * 13.7 + length * 1.8 - age * 6.8;

            if (gender.Equals("Female")) bmr = 655 + 74 * 9.6 + 170 * 1.8 - 25 * 4.7;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(QueryString, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@age", age);
                    command.Parameters.AddWithValue("@length", length);
                    command.Parameters.AddWithValue("@weight", weight);
                    command.Parameters.AddWithValue("@gender", gender);
                    command.Parameters.AddWithValue("@bmi", bmi);
                    command.Parameters.AddWithValue("@bmr", bmr);

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
            using (var connection2 = new SqlConnection(_connectionString))
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