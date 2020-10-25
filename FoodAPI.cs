using ClientMyFitnessApp;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;


namespace ServerMyFitnessApp
{
    public static class FoodApi
    {

        /* FoodApi Class
        * Contains the code we need to to retreive details from the API
        * 
        */
        public static List<FoodItem> RetrieveFromFoodApi(string searchingFor)
        {
            //Declaring variables
            var arrayListFood = new List<FoodItem>();


            //API Stuff, searchingfor is the food we want. For example: Cake
            var client = new RestClient("https://edamam-food-and-grocery-database.p.rapidapi.com/parser?ingr=" + searchingFor);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "edamam-food-and-grocery-database.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "98c6907f94msh24117bf6f31b20ep1ed75fjsn320c530b2a08");
            var response = client.Execute(request);
            string responseString = response.Content;

            //Deserializing the json object
            var myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseString);

            // Done with retrieving food

            for (int i = 0; i < myDeserializedClass.hints.Count; i++)
            {
                //Creating a food item and adding it to the list we return. 
                string foodName = myDeserializedClass.hints[i].food.label;
                string foodCategory = myDeserializedClass.hints[i].food.category;
                double foodCalories = myDeserializedClass.hints[i].food.nutrients.ENERC_KCAL;
                double foodFat = myDeserializedClass.hints[i].food.nutrients.FAT;
                double foodFiber = myDeserializedClass.hints[i].food.nutrients.FIBTG;
                string foodContents = myDeserializedClass.hints[i].food.foodContentsLabel;

                var tempFoodItem = new FoodItem(foodName, foodCategory, foodCalories, foodFat, foodFiber, foodContents);
                arrayListFood.Add(tempFoodItem);
            }

            return arrayListFood;
        }

    }
}
