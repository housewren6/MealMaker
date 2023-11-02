using Floggr.Data;
using Floggr.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Floggr.Controllers
{

    public class MealMakerController : Controller
    {
        private readonly FloggrContext _context;
        OpenAIController oac = new OpenAIController();
        private IQueryable<SelectFood> selectFoodNameCatResults;
        private string[] arrProteins = { "Beef Products", "Finfish and Shellfish Products", "Legumes and Legume Products", "Nut and Seed Products", "Pork Products", "Poultry Products", "Sausages and Luncheon Meats" };
        private string[] arrGrains = { "Baked Products", "Cereal Grains and Pasta" };
        private string[] arrDairyFats = { "Dairy and Egg Products", "Beverages", "Fats and Oils" };
        private string[] arrFruits = { "Fruits and Fruit Juices" };
        private string[] arrVegetables = { "Vegetables and Vegetable Products", "Soups, Sauces, and Gravies" };
        public MealMakerController(FloggrContext context)
        {
            _context = context;
            //BUILD ONE TABLE WITH TWO COLUMNS, ONE FOR FOOD NAME AND ONE FOR FOOD DESCRIPTION. 
            selectFoodNameCatResults = _context.FoundationFoods.Join(
            _context.FoodCategories,
            food => food.foodCategoryID,
            foodCat => foodCat.foodCategoryID,
            (food, foodCat) =>
            new SelectFood { foodName = food.description, foodID = food.foundationFoodID, foodCat = foodCat.description });
        }

        //GET: Build a Meal
        [HttpGet]
        public async Task<IActionResult> BuildMeal()
        {
            if (TempData["ErrorMessage"] != null)
            {
                //RETRIEVE AND DISPLAY ERROR MESSAGES 
                string errorMessage = TempData["ErrorMessage"].ToString();
                ViewBag.ErrorMessage = errorMessage;
            }

            MealMakerBuildView mealMakerBuildView = new MealMakerBuildView()
            {
                selectProteins = new SelectList(await selectFoodNameCatResults.Where(c => arrProteins.Contains(c.foodCat)).ToListAsync(), "foodID", "foodName"),
                //listProteins = new List(await selectFoodNameCatResults.Where(c => arrProteins.Contains(c.foodCat)).ToListAsync(), "foodID", "foodName"),
                selectGrains = new SelectList(await selectFoodNameCatResults.Where(c => arrGrains.Contains(c.foodCat)).ToListAsync(), "foodID", "foodName"),
                selectDairyFats = new SelectList(await selectFoodNameCatResults.Where(c => arrDairyFats.Contains(c.foodCat)).ToListAsync(), "foodID", "foodName"),
                selectFruits = new SelectList(await selectFoodNameCatResults.Where(c => arrFruits.Contains(c.foodCat)).ToListAsync(), "foodID", "foodName"),
                selectVegetables = new SelectList(await selectFoodNameCatResults.Where(c => arrVegetables.Contains(c.foodCat)).ToListAsync(), "foodID", "foodName")
            };
            return View(mealMakerBuildView);
        }

        //POST: Build a Meal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuildMeal(MealMakerBuildView mmbv)
        {
            //GET USER INPUT INGREDIENTS AND ADD FOODID TO INPUTFOODS LIST, CALL API 
            var propertiesToIterate = new List<List<string>>
                {
                    mmbv.Proteins,
                    mmbv.Grains,
                    mmbv.DairyFats,
                    mmbv.Fruits,
                    mmbv.Vegetables
                };
            var inputFoods = propertiesToIterate.SelectMany(x => x).ToList();
            MealView mealView = new MealView()
            {
                MealInstructions = await WriteRecipeAICall(inputFoods)
            };

            if (ModelState.IsValid)
            { return View("ViewMeal", mealView); }
            else
            { return RedirectToAction("BuildMeal"); }
        }

        //[HttpGet]
        //public async Task<IActionResult> ViewRandomMeal()
        //{
        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult> ViewRandomMeal()
        {
            //GET RANDOM INGREDIENTS, 1:1:1:1:3 PR:GR:DF:FR:VG, AND ADD FOODID TO MASTER LIST FOR AI CALL
            var selectedFoods = new List<string>();
            var protein = await selectFoodNameCatResults
                    .Where(c => arrProteins.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            if (protein != null)
            {
                selectedFoods.Add(protein.foodID.ToString());
            }
            var grain = await selectFoodNameCatResults
                    .Where(c => arrGrains.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            if (grain != null)
            {
                selectedFoods.Add(grain.foodID.ToString());
            }
            var dairyfat = await selectFoodNameCatResults
                    .Where(c => arrDairyFats.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            if (dairyfat != null)
            {
                selectedFoods.Add(dairyfat.foodID.ToString());
            }
            var fruits = await selectFoodNameCatResults
                    .Where(c => arrFruits.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync();
            if (fruits != null)
            {
                selectedFoods.Add(fruits.foodID.ToString());
            }
            var vegetables = await selectFoodNameCatResults
                    .Where(c => arrVegetables.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(3).ToListAsync();
            if (vegetables != null)
            {
                foreach (var veg in vegetables)
                {
                    selectedFoods.Add(veg.foodID.ToString());
                }
            }
            //FINALLY, BUILD VIEWMODEL BY CALLING API CALL
            MealMakerView mealMakerView = new MealMakerView()
            {
                RecipeInstructions = await WriteRecipeAICall(selectedFoods)
            };
            
            if (ModelState.IsValid)
            // { return View("ViewRandomMeal", mealMakerView); }
            { return View(mealMakerView); }
            else
            { return RedirectToAction("BuildMeal"); }
        }

        public async Task<string> WriteRecipeAICall(List<string> foods)
        {
            //model state expected: ingredients[& modifiers] to build recipe. 
            //calls ai api to make recipe
            //returns one string of ingredients, amounts, instructions, recipe name 
            var api = new OpenAI_API.OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            string AIPrompt = "";
            if (foods != null)
            {
                AIPrompt = "Your task is to generate a unique recipe that includes the following ingredients: ";
                foreach (var food in foods)
                {
                    AIPrompt += "(" + await _context.FoundationFoods.Where(f => f.foundationFoodID == int.Parse(food)).Select(f => f.description).FirstOrDefaultAsync() + "), ";
                }
                AIPrompt = AIPrompt.Substring(0, AIPrompt.Length - 2);

            }
            AIPrompt +=
            """
                Please generate a unique recipe and the simple steps to safely prepare it.
                Please provide the meal name, the ingredients full names and their amount, and the simple, clear step-by-step instructions to safely prepare it. 
                Please ensure recipes are distinct from the examples provided.
                Please ensure the meal includesat least as many ingredients from the provided ingredients, if any are given, that work appropriately in the meal. 
                Additional ingredients can be suggested if they complement the meal.

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

            //HANDLE ERROR IF TOO MANY FOODS ARE SELECTED/MAX TOKEN EXCEEDED
            if (AIPrompt.Length > 2900)
            {
                ModelState.AddModelError("StringList", "Whoops! Too many foods were selected. Please select less total foods.");
                TempData["ErrorMessage"] = "Whoops! Too many foods were selected. Please select less total foods.";
            }

            if (ModelState.IsValid)
            {
                //HANDLE ERROR IF TOO MANY REQUESTS ARE MADE AT ONCE
                try
                {
                    var apiresult = await api.Completions.CreateCompletionAsync(prompt: AIPrompt, model: OpenAI_API.Models.Model.DavinciText, temperature: 0.8, max_tokens: 1024);
                    return apiresult.ToString();
                }
                catch (HttpRequestException ex)
                {
                    if (ex.Message.Contains("rate_limit_exceeded"))
                    {
                        ModelState.AddModelError("StringList", "Whoops! Too many requests made within the time limit. Please wait up to 20 seconds. If this issue persists, please use our Contact page to report this issue.");
                        TempData["ErrorMessage"] = "Whoops! Too many requests made within the time limit. Please wait up to 20 seconds. If this issue persists, please use our Contact page to report this issue.";
                    }
                    else
                    {
                        ModelState.AddModelError("StringList", "Whoops! An error occured with your request. Please try again. If this issue persists, please use our Contact page to report this issue.");
                        TempData["ErrorMessage"] = "Whoops! An error occured with your request. Please try again. If this issue persists, please use our Contact page to report this issue.";
                    }
                }
                catch //(Exception ex)
                {
                    ModelState.AddModelError("StringList", "Whoops! An unexpected error occured. Please try again. If this issue persists, please use our Contact page to report this issue.");
                    TempData["ErrorMessage"] = "Whoops! An unexpected error occured. Please try again. If this issue persists, please use our Contact page to report this issue.";
                }
            }
            return "";
        }
    }
}
