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
		 
		// GET: FoundationFoods
		public async Task<IActionResult> ListFoundationFoods(string sortOrder, int? page, int? pageSize, string searchTerm)
        {
            ViewBag.FoodNameSort = String.IsNullOrEmpty(sortOrder) ? "foodName" : "foodName";
            ViewBag.FoodCatSort = sortOrder == "foodCat" ? "foodCat" : "foodCat";
            ViewBag.CurrentSort = sortOrder;
            ViewData["CurrentSort"] = sortOrder;

            int itemsPerPage = pageSize ?? 10;
            int pageNumber = page ?? 1;
            ViewData["ItemsPerPage"] = itemsPerPage;

            //BUILD ONE TABLE WITH TWO COLUMNS, ONE FOR FOOD NAME AND ONE FOR FOOD DESCRIPTION. 
            var selectFoodNameCatResults = _context.FoundationFoods.Join(
            _context.FoodCategories,
            food => food.foodCategoryID,
            foodCat => foodCat.foodCategoryID,
            (food, foodCat) =>
            new SelectFood { foodName = food.description, foodID = food.foundationFoodID, foodCat = foodCat.description });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                selectFoodNameCatResults = selectFoodNameCatResults
                    .Where(food => food.foodName.Contains(searchTerm));
            }

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

            if (itemsPerPage == 0)
            {
                //Get all records, no pagination
                var allFoundationFoods = selectFoodNameCatResults.ToList();
                var mmv = new MealMakerView()
                {
                    AllFoundationFoods = await selectFoodNameCatResults.ToListAsync(),
                    HasPreviousPage = false, 
                    HasNextPage = false, 
                    PageIndex = 1,
                    PageCount = 1, 
                    PageSize = allFoundationFoods.Count 
                };

                return View(mmv);
            }
            int pageCount = await selectFoodNameCatResults.CountAsync();
            var paginatedFoods = new PaginatedList<SelectFood>(selectFoodNameCatResults.ToList(), pageCount, pageNumber, itemsPerPage);
            MealMakerView mealMakerView = new MealMakerView()
            {
                AllFoundationFoods = paginatedFoods.Items,
                HasPreviousPage = paginatedFoods.HasPreviousPage,
                HasNextPage = paginatedFoods.HasNextPage,
                PageIndex = paginatedFoods.PageIndex,
                PageCount = paginatedFoods.TotalPages,
                PageSize = itemsPerPage
            };
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
					.OrderBy(f => Guid.NewGuid()).Take(1).ToListAsync();
            var vegetables = await selectFoodNameCatResults
					.Where(c => arrVegetables.Contains(c.foodCat))
					.OrderBy(f => Guid.NewGuid()).Take(3).ToListAsync();

			var api = new OpenAI_API.OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            string AIPrompt =
               $"""
                   Your task is to generate a unique recipe that includes the following ingredients: 
                   ({protein.foodName}), ({grain.foodName}), ({dairyfat.foodName}), ){fruits[0].foodName}),  
                   ({vegetables[0].foodName}), ){vegetables[1].foodName}), ){vegetables[2].foodName}).
                   Please provide the meal name, the ingredients full names and their amount, and the simple, clear step-by-step instructions to safely prepare it. 
                   Please ensure recipes are distinct from the examples provided but include ingredients from the provided list.

                   Examples:
                   Meal:
                   Eggplant Parmesan

                   Ingredients:
                   1lb purple eggplant
                   1 cup breadcrumbs
                   1 egg 
                   1 lb mozarella 
                   1 pt tomato sauce 
                   1 lb pasta, any type

                   Instructions:
                   1. Slice the eggplant into 1" rounds.
                   2. Prepage egg wash by whisking egg and a splash of water together in a bowl. 
                   3. Prepare eggplant by dredging in egg wash and coating in bread crumbs. Place the prepared eggplant on a lined baking sheet. Season the breadcrumbs or eggplant as desired.
                   4. Bake in oven at 450 for 30 minutes. 
                   5. While the eggplant cooks, prepare pasta in boiled water per instructions on box.
                   6. When the pasta is finished and the eggplant is cooked, place slices of mozarella on the eggplant to melt for 5 minutes under a broil, and mix the tomato sauce in with the pasta.
                   7. Serve the pasta and eggplant together and enjoy. 

                   Meal: 
                   Fried Rice

                   Ingredients:
                   1 lb rice, precooked
                   1 onion
                   1 lb carrots
                   1 napa cabbage
                   4 eggs

                   Instructions: 
                   1. Slice the carrots and saute with oil in a pan or wok on medium heat. 
                   2. Dice the onions, and add to the pan when the carrots are about halfway cooked. 
                   3. Shred the cabbage, and add to the pan to fry on medium-high heat. Season vegetables as desired.
                   4. Add the rice and eggs to the pan, frying and mixing all together until all eggs have cooked. 
                   5. Serve with a sauce if desired, and enjoy.                
                    
               """;
                
            var apiresult = await api.Completions.CreateCompletionAsync(prompt: AIPrompt,
                model: OpenAI_API.Models.Model.DavinciText, temperature: 0.8, max_tokens: 2000);
            string resultString = apiresult.ToString();

            //FINALLY, BUILD VIEWMODEL
            MealMakerView mealMakerView = new MealMakerView() {
                ////foods = proteinResults.ToList(),
                //Protein = protein,
                //Grain = grain,
                //DairyFat = dairyfat,
                //Fruits = fruits,
                //Vegetables = vegetables,
                //RecipeName = recipeName,
                //RecipeIngredients = ingredients,
                //RecipeInstructions = instructions
                RecipeInstructions = resultString                
            };        
            return View(mealMakerView);
        }       
    }
}
