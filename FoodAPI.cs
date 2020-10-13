using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClientMyFitnessApp;
using Newtonsoft.Json;
using RestSharp;


namespace ServerMyFitnessApp
{
    public static class FoodAPI
    {

        public static List<FoodItem> RetrieveFromFoodAPI(string SearchingFor)
        {
            var ArrayListFood = new List<FoodItem>();
            //API Stuff
            var client = new RestClient("https://edamam-food-and-grocery-database.p.rapidapi.com/parser?ingr=" + SearchingFor);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "edamam-food-and-grocery-database.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "98c6907f94msh24117bf6f31b20ep1ed75fjsn320c530b2a08");
            IRestResponse response = client.Execute(request);
            string responsestring = response.Content;
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responsestring);
            // Done with retreiving food

            for (int i = 0; i < myDeserializedClass.hints.Count; i++)
            {
                string foodname = myDeserializedClass.hints[i].food.label;
                string foodcategory = myDeserializedClass.hints[i].food.category;
                double foodkcal = myDeserializedClass.hints[i].food.nutrients.ENERC_KCAL;
                double foodfat = myDeserializedClass.hints[i].food.nutrients.FAT;
                double foodfiber = myDeserializedClass.hints[i].food.nutrients.FIBTG;
                string foodcontents = myDeserializedClass.hints[i].food.foodContentsLabel;

                FoodItem TempFoodItem = new FoodItem(foodname, foodcategory, foodkcal, foodfat, foodfiber, foodcontents);

                ArrayListFood.Add(TempFoodItem);
            }

            return ArrayListFood;
        }

    }
}
