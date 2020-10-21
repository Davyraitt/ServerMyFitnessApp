using ClientMyFitnessApp;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;


namespace ServerMyFitnessApp
{
    public static class FoodApi
    {

        public static List<FoodItem> RetrieveFromFoodApi(string searchingFor)
        {
            var arrayListFood = new List<FoodItem>();
            //API Stuff
            var client = new RestClient("https://edamam-food-and-grocery-database.p.rapidapi.com/parser?ingr=" + searchingFor);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "edamam-food-and-grocery-database.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "98c6907f94msh24117bf6f31b20ep1ed75fjsn320c530b2a08");
            var response = client.Execute(request);
            string responseString = response.Content;
            var myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseString);
            // Done with retrieving food

            for (int i = 0; i < myDeserializedClass.hints.Count; i++)
            {
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
