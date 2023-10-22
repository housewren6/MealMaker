using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RootFoundationFoods;

namespace Floggr.Models.ViewModels
{
    public class MealMakerBuildView// : Meal
    {
        public MealMakerBuildView() {
            Proteins = new List<string>();
            Grains = new List<string>();
            DairyFats = new List<string>();
            Fruits = new List<string>();
            Vegetables = new List<string>();

        }
        public List<string> Proteins { get; set; }

        public List<string> Grains { get; set; }
        public List<string> DairyFats { get; set; }
        public List<string> Fruits { get; set; }
        public List<string> Vegetables { get; set; }
        public List<string> Modifiers { get; set; }
        //public List<SelectFood> listProteins { get; set; }

        public SelectList selectProteins { get; set; }
        public SelectList selectGrains { get; set; }
        public SelectList selectDairyFats { get; set; }
        public SelectList selectFruits { get; set; }
        public SelectList selectVegetables { get; set; }
        
        public SelectList Foods { get; set; }
    }
    
}
