using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMyFitnessApp
{
    
    public class FoodItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Kcal { get; set; }

        public double Fat { get; set; }
        public double Fibers { get; set; }
        public string FoodContents { get; set; }


        public FoodItem(string name, string category, double kcal, double fat, double fibers, string foodContents)
        {
            Name = name;
            Category = category;
            Kcal = kcal;
            Fat = fat;
            Fibers = fibers;
            FoodContents = foodContents;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Category)}: {Category}, {nameof(Kcal)}: {Kcal}, {nameof(Fat)}: {Fat}, {nameof(Fibers)}: {Fibers}, {nameof(FoodContents)}: {FoodContents}";
        }
    }

    public class Nutrients
    {
        public double ENERC_KCAL { get; set; }
        public double PROCNT { get; set; }
        public double FAT { get; set; }
        public double CHOCDF { get; set; }
        public double FIBTG { get; set; }
    }

    public class Food
    {
        public string foodId { get; set; }
        public string uri { get; set; }
        public string label { get; set; }
        public Nutrients nutrients { get; set; }
        public string category { get; set; }
        public string categoryLabel { get; set; }
        public string image { get; set; }
    }

    public class Parsed
    {
        public Food food { get; set; }
    }

    public class Nutrients2
    {
        public double ENERC_KCAL { get; set; }
        public double PROCNT { get; set; }
        public double FAT { get; set; }
        public double CHOCDF { get; set; }
        public double FIBTG { get; set; }
    }

    public class Food2
    {
        public string foodId { get; set; }
        public string uri { get; set; }
        public string label { get; set; }
        public Nutrients2 nutrients { get; set; }
        public string category { get; set; }
        public string categoryLabel { get; set; }
        public string image { get; set; }
        public string foodContentsLabel { get; set; }
    }

    public class Measure
    {
        public string uri { get; set; }
        public string label { get; set; }
        public List<List<Qualified>> qualified { get; set; }
    }

    public class Hint
    {
        public Food2 food { get; set; }
        public List<Measure> measures { get; set; }
    }

    public class Next
    {
        public string title { get; set; }
        public string href { get; set; }
    }

    public class Links
    {
        public Next next { get; set; }
    }

    public class Root
    {
        public string text { get; set; }
        public List<Parsed> parsed { get; set; }
        public List<Hint> hints { get; set; }
        public Links _links { get; set; }
    }

    public class Qualified
    {
        public String uri { get; set; }
        public String label { get; set; }
    }
}
