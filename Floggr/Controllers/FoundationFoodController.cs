using Floggr.Data;
using Floggr.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using RootFoundationFoods;
using PagedList.EntityFramework;
using Floggr.Code;

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
        public async Task<IActionResult> ListFoundationFoods(string sortOrder, int? page)
        {
            ViewBag.FoodNameSort = String.IsNullOrEmpty(sortOrder) ? "foodName" : "foodName";
            ViewBag.FoodCatSort = sortOrder == "foodCat" ? "foodCat" : "foodCat";
            int pageSize = 20;
            int ?pageNumber = (page ?? 1);
            
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

            //FINALLY, BUILD VIEWMODEL
            MealMakerView mealMakerView = new MealMakerView() {
                //foods = proteinResults.ToList(),
                Protein = await selectFoodNameCatResults
                    .Where(c => arrProteins.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync(),
                Grain = await selectFoodNameCatResults
                    .Where(c => arrGrains.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync(),
                DairyFat = await selectFoodNameCatResults
                    .Where(c => arrDairyFats.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(1).FirstOrDefaultAsync(),
                Fruits = await selectFoodNameCatResults
                    .Where(c => arrFruits.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(2).ToListAsync(),
                Vegetables = await selectFoodNameCatResults
                    .Where(c => arrVegetables.Contains(c.foodCat))
                    .OrderBy(f => Guid.NewGuid()).Take(3).ToListAsync()
            };
            return View(mealMakerView);
        }
    }
}
