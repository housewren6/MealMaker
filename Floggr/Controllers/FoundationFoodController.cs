using Floggr.Data;
using Floggr.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using RootFoundationFoods;
using Floggr.Code;
using Floggr.Controllers;
using OpenAI_API.Completions;

namespace Floggr.Controllers
{
    public class FoundationFoodController : Controller
    {

        private readonly FloggrContext _context;

        public FoundationFoodController(FloggrContext context)
        {
            _context = context;
        }
        //REFRESH PAGE
        public async Task<RedirectToActionResult> RefreshPage() 
        {
            return RedirectToAction();
        }
		OpenAIController oac = new OpenAIController();
		// GET: FoundationFoods
		public async Task<IActionResult> ListFoundationFoods(string sortOrder, int? page)
        {
            ViewBag.FoodNameSort = String.IsNullOrEmpty(sortOrder) ? "foodName" : "foodName";
            ViewBag.FoodCatSort = sortOrder == "foodCat" ? "foodCat" : "foodCat";
            //int pageSize = 20;
            //int ?pageNumber = (page ?? 1);
            
           //BUILD ONE TABLE WITH TWO COLUMNS, ONE FOR FOOD NAME AND ONE FOR FOOD DESCRIPTION. 
            var selectFoodNameCatResults = _context.FoundationFoods.Join(
            _context.FoodCategories,
            food => food.foodCategoryID,
            foodCat => foodCat.foodCategoryID,
            (food, foodCat) =>
            new SelectFood { foodName = food.description, foodID = food.foundationFoodID, foodCat = foodCat.description });
            switch (sortOrder)
            {
                case "foodName":
                    selectFoodNameCatResults = selectFoodNameCatResults.OrderBy(s => s.foodName);
                    break;
                case "foodCat":
                    selectFoodNameCatResults = selectFoodNameCatResults.OrderBy(s => s.foodCat);
                    break;
                default:
                    selectFoodNameCatResults = selectFoodNameCatResults.OrderBy(s => s.foodID);
                    break;
            }
            //var allFoodPaginatedList = PaginatedList<SelectFood>.CreateAsync(selectFoodNameCatResults.AsNoTracking(), pageNumber ?? 1, pageSize);
            //ListInfo listInfo = new ListInfo();
            MealMakerView mealMakerView = new MealMakerView()
            {
                AllFoundationFoods = await selectFoodNameCatResults.ToListAsync()
                //AllFoundationFoods = await selectFoodNameCatResults.ToPagedListAsync(pageNumber, pageSize)
                //AllFoundationFoods = await allFoodPaginatedList,
               // HasPreviousPage = listInfo.hasPreviousPage,
                //HasNextPage = listInfo.hasNextPage,
                //PageIndex = listInfo.pageNumber

            };
            //return View();
            //return View(await _context.FoundationFoods.ToListAsync());
            return View(mealMakerView);
        }
        // GET: MealMaker view with all new random results
        public async Task<IActionResult> MealMaker()
        {           
			string[] arrProteins = {"Beef Products","Finfish and Shellfish Products", "Legumes and Legume Products",
                "Nut and Seed Products", "Pork Products", "Poultry Products", "Sausages and Luncheon Meats" };
            string[] arrGrains = { "Baked Products", "Cereal Grains and Pasta" };
            string[] arrDairyFats = { "Dairy and Egg Products", "Beverages", "Fats and Oils" };
            string[] arrFruits = { "Fruits and Fruit Juices" };
			string[] arrVegetables = { "Vegetables and Vegetable Products", "Soups, Sauces, and Gravies" };

            //BUILD ONE TABLE WITH TWO COLUMNS, ONE FOR FOOD NAME AND ONE FOR FOOD DESCRIPTION. 
            var selectFoodNameCatResults = _context.FoundationFoods.Join(
                _context.FoodCategories,
                food => food.foodCategoryID,
                foodCat => foodCat.foodCategoryID,
                (food, foodCat) =>
                new SelectFood { foodName = food.description, foodID = food.foundationFoodID, foodCat = foodCat.description });

            var protein = await selectFoodNameCatResults
					.Where(c => arrProteins.Contains(c.foodCat))
					.OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            var grain = await selectFoodNameCatResults
					.Where(c => arrGrains.Contains(c.foodCat))
					.OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            var dairyfat = await selectFoodNameCatResults
					.Where(c => arrDairyFats.Contains(c.foodCat))
					.OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            var fruits = await selectFoodNameCatResults
					.Where(c => arrFruits.Contains(c.foodCat))
					.OrderBy(f => Guid.NewGuid()).Take(2).ToListAsync();
            var vegetables = await selectFoodNameCatResults
					.Where(c => arrVegetables.Contains(c.foodCat))
					.OrderBy(f => Guid.NewGuid()).Take(3).ToListAsync();



			//var oac = new OpenAIController();
			var api = new OpenAI_API.OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            string AIPrompt = $"Suggest a meal and the simple steps to prepare it with as many of the the following ingredients as " +
                $"possible: {protein.foodName}, {grain.foodName}, {dairyfat.foodName}, {fruits[0].foodName}, {fruits[1].foodName}, " +
                $"{vegetables[0].foodName}, {vegetables[1].foodName}, {vegetables[2].foodName}. ";
           
                //"""
                //Suggest a meal and the simple steps to prepare it with the following ingredients: . 
                //""";
                //Meal: Fried Rice
                //Ingredients: Rice, Onion, Celery, Carrot, Egg
                //Steps: 1. Cook rice as the bag instructs. Let cool completely.
                //2. Clean and chop the vegetables into a small dice.
                //3. Fry the vegetables over medium high heat in a wok until translucent.
                //4. Add the rice and egg to the wok, cooking egg completely through.
                //5. Serve and enjoy.
                
                //Meal: Spaghetti and Marinara
                //Ingredients: Spaghetti pasta, tomato sauce, eggplant, onions, mozarella cheese
                //Steps: 1. Chop eggplant into 1/2\" thick rounds, season and bake at 450 in an oven for 30 minutes. 
                //2. Cook spaghetti pasta as instructed on box. Boil until al dente then strain.
                //3. Chop and saute the onions over medium heat until translucent.
                //4. Add tomato sauce to onions and season to taste. 
                //5. Combine cooked eggplant, spaghetti pasta, and sauce in a large pot. Cover with mozarella cheese and let melt.
                //6. Serve and enjoy.
                
                //Meal:
                //Ingredients:
                //""";
			var apiresult = await api.Completions.CreateCompletionAsync(prompt: AIPrompt,
                model: OpenAI_API.Models.Model.DavinciText, temperature: 0.8, max_tokens: 1024);
            string resultString = apiresult.ToString();
            int ingredientsIndex = resultString.IndexOf("Ingredients:");
			int instructionsIndex = resultString.IndexOf("Instructions:");
            if( ingredientsIndex <= 0 || instructionsIndex <= 0 ) { return View(); }
			string recipeName = resultString.Substring(0, ingredientsIndex);
            string ingredients = resultString.Substring(ingredientsIndex, instructionsIndex-ingredientsIndex); 
			string instructions = resultString.Substring(instructionsIndex);


			// should print something starting with "Three"
			//FINALLY, BUILD VIEWMODEL
			MealMakerView mealMakerView = new MealMakerView() {
                //foods = proteinResults.ToList(),
                Protein = protein,
                Grain = grain,
                DairyFat = dairyfat,
                Fruits = fruits,
                Vegetables = vegetables,
                RecipeName = recipeName,
                RecipeIngredients = ingredients,
                RecipeInstructions = instructions
                //oac.UseOpenAI("Suggest a vegetarian meal and the simple steps to prepare it with five ingredients. " +
                //"\r\n\r\n    Meal: Fried Rice\r\n    " +
                //"Ingredients: Rice, Onion, Celery, Carrot, Egg\r\n    " +
                //"Steps: 1. Cook rice as the bag instructs. Let cool completely.\r\n    " +
                //"2. Clean and chop the vegetables into a small dice.\r\n    " +
                //"3. Fry the vegetables over medium high heat in a wok until translucent. \r\n    " +
                //"4. Add the rice and egg to the wok, cooking egg completely through. \r\n    " +
                //"5. Serve and enjoy.\r\n\r\n    " +
                //"" +
                //"Meal: Spaghetti and Marinara\r\n    " +
                //"Ingredients: Spaghetti pasta, tomato sauce, eggplant, onions, mozarella cheese\r\n    " +
                //"Steps: 1. Chop eggplant into 1/2\" thick rounds, season and bake at 450 in an oven for 30 minutes. \r\n    " +
                //"2. Cook spaghetti pasta as instructed on box. Boil until al dente then strain.\r\n    " +
                //"3. Chop and saute the onions over medium heat until translucent.\r\n    " +
                //"4. Add tomato sauce to onions and season to taste. \r\n    " +
                //"5. Combine cooked eggplant, spaghetti pasta, and sauce in a large pot. Cover with mozarella cheese and let melt.\r\n    " +
                //"6. Serve and enjoy.\r\n     \r\n    " +
                //"" +
                //"Meal: ")
            };

           


            return View(mealMakerView);
        }
    }
}
