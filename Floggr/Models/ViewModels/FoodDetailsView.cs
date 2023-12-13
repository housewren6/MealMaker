namespace Floggr.Models.ViewModels
{
    public class FoodDetailsView
    {

        public int FoodDetailsViewID { get; set; }
        public string FoodName { get; set; }
        public List<FoodNutrientDetail> FoodNutrientDetails { get; set;}
    }
}
