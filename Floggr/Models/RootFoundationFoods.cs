// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RootFoundationFoods
{  
        public class FoodCategory
    {
        public string description { get; set; }
        [Key]
        public int foodCategoryID { get; set; }
       // public int foundationFoodID { get; set; }
        //[ForeignKey("foundationFoodID")]
        //public FoundationFood FoundationFood { get; set; }
    }

    public class FoodNutrient
    {
        public string type { get; set; }
        [Key]
        public int foodNutrientID { get; set; }
        public Nutrient nutrient { get; set; }
        public int dataPoints { get; set; }
        public FoodNutrientDerivation foodNutrientDerivation { get; set; }
        public double median { get; set; }
        public double amount { get; set; }
        public double? max { get; set; }
        public double? min { get; set; }
    }
 
    public class FoodNutrientDerivation
    {
        [Key]
        public int foodNutrientDerivationID { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public FoodNutrientSource foodNutrientSource { get; set; }
    }

    public class FoodNutrientSource
    {
        [Key]
        public int foodNutrientSourceID { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }

    public class FoodPortion
    {
        [Key]
        public int foodPortionID { get; set; }
        public double value { get; set; }
        public MeasureUnit measureUnit { get; set; }
        public string modifier { get; set; }
        public double gramWeight { get; set; }
        public int sequenceNumber { get; set; }
        public int minYearAcquired { get; set; }
        public double amount { get; set; }
    }

    public class FoundationFood
    {
        [Key]
        public int foundationFoodID { get; set; }
        public string foodClass { get; set; }
        public string description { get; set; }
        public IList<FoodNutrient> foodNutrients { get; set; }
        //not a single food has an attribute....removing
        //public IQueryable<FoodAttribute> foodAttributes { get; set; }
        //public class FoodAttribute
        //{
        //    public int id { get; set; }
        //    public string[] foodAttributeObject { get; set; }
        //}
        public IList<NutrientConversionFactor> nutrientConversionFactors { get; set; }
        public bool isHistoricalReference { get; set; }
        public int? ndbNumber { get; set; }
        public string dataType { get; set; }
        public int foodCategoryID { get; set; }
        [ForeignKey("foodCategoryID")]
        public FoodCategory foodCategory { get; set; }
        public int fdcId { get; set; }
        public IList<FoodPortion> foodPortions { get; set; }
        public string publicationDate { get; set; }
        public IList<InputFood> inputFoods { get; set; }
        public string scientificName { get; set; }
    }

    public class InputFood
    {
        [Key]
        public int inputFoodsID { get; set; }
        public string foodDescription { get; set; }
        public InputFood2 inputFood { get; set; }
    }

    public class InputFood2
    {
        [Key]
        public int inputFoods2ID { get; set; }
        public string foodClass { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public FoodCategory foodCategory { get; set; }
        public int? fdcId { get; set; }
        public string publicationDate { get; set; }
    }

    public class MeasureUnit
    {
        [Key]
        public int measureUnitID { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class Nutrient
    {
        [Key]
        public int nutrientID { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public string unitName { get; set; }
    }

    public class NutrientConversionFactor
    {
        [Key]
        public int nutrientConversionFactorID { get; set; }
        public string type { get; set; }
        public double proteinValue { get; set; }
        public double fatValue { get; set; }
        public double carbohydrateValue { get; set; }
        public double? value { get; set; }
    }

    public class Root
    {
        [Key]
        public int rootID { get; set; }
        public IList<FoundationFood> FoundationFoods { get; set; }
    }

}