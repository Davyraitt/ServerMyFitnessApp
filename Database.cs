using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using ClientMyFitnessApp;

namespace ServerMyFitnessApp
{
    public static class Database
    {

        public static bool WriteFoodToDatabase(List<FoodItem> completeFoodList)
        {
            Console.WriteLine("WRITING TO DATABASE");


            string connectionString = "Data Source=SQLServer;" +
                                      "Initial Catalog=FitnessDatabase; Integrated Security=False;" +
                                      "User id=sa;" +
                                      "Password=Avans2020;";

            string connectionString2 = "Data Source=SQLServer;" +
                                      "Initial Catalog=FitnessDatabase; Integrated Security=False;" +
                                      "User id=sa;" +
                                      "Password=Avans2020;";


            //clearing the database for testing purposes
            using (SqlConnection connection2 = new SqlConnection(connectionString2))
            {
                using (SqlCommand command = new SqlCommand("DELETE FROM dbo.food", connection2))
                {
                    connection2.Open();
                    int result2 = command.ExecuteNonQuery();
            
                    // Check Error
                    if (result2 < 0)
                        return false;
                    Console.WriteLine("Error inserting data into Database!");
                }
            }

            //refilling the database

            string QueryString =
                "INSERT INTO dbo.Food (Name,Category,Kcal,Fat,Fiber,Contents) VALUES (@name,@category,@kcal,@fat,@fiber,@contents)";

            for (int i = 0; i < completeFoodList.Count; i++)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(QueryString, connection))
                    {
                        command.Parameters.AddWithValue("@name", completeFoodList[i].Name);
                        command.Parameters.AddWithValue("@category", completeFoodList[i].Category);
                        command.Parameters.AddWithValue("@kcal", completeFoodList[i].Kcal);
                        command.Parameters.AddWithValue("@fat", completeFoodList[i].Fat);
                        command.Parameters.AddWithValue("@fiber", completeFoodList[i].Fibers);
                        command.Parameters.AddWithValue("@contents", "FoodContent");
                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        // Check Error
                        Console.WriteLine("Done writing item " + completeFoodList[i].Name + " to the database!");
                    }
                }
            }

            return true;
        }
    }
}