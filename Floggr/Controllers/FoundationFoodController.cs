using Floggr.Code;
using Floggr.Data;
using Floggr.Models;
using Floggr.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: FoodDetails/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodDetailsResults = _context.FoundationFoods
           .Where(ff => ff.foundationFoodID == id)
           .SelectMany(ff => ff.foodNutrients) // Flatten the foodNutrients list
           .Join(_context.Nutrients, fn => fn.foodNutrientID, n => n.nutrientID, (fn, n) => new { fn, n })
           //.Join(_context.FoodNutrientDerivations, fn_n => fn_n.fn.foodNutrientDerivationID, fnd => fnd.foodNutrientDerivationID, (fn_n, fnd) => new { fn_n.fn, fn_n.n, fnd })
           //.Join(_context.FoodNutrientSource, fn_n_fnd => fn_n_fnd.fnd.foodNutrientSourceID, fns => fns.foodNutrientSourceID, (fn_n_fnd, fns) => new { fn_n_fnd.fn, fn_n_fnd.n, fn_n_fnd.fnd, fns })
           .Where(result => /*result.fn.foundationFoodID == id && */result.fn.amount >= 0.5)
           .Select(result => new FoodNutrientDetail
           {
               NutrientName = result.n.name,
               Rank = result.n.rank,
               //FoodDescription = result.fn.foundationFood.description,
               Amount = result.fn.amount,
               UnitName = result.n.unitName
           })
           .Distinct()
           .OrderBy(result => result.Rank);

            //ADD ERROR HANDLING
            if (foodDetailsResults == null)
            {
                return NotFound();
            }

            var foodName = _context.FoundationFoods
                .Where(ff => ff.foundationFoodID == id)
                .Select(ff => ff.description);

            //ADD ERROR HANDLING
            if (foodName == null)
            {
                return NotFound();
            }

            FoodDetailsView fd = new FoodDetailsView
            {
                FoodName = await foodName.FirstOrDefaultAsync(),
                FoodNutrientDetails = await foodDetailsResults.ToListAsync()
            };

            return View(fd);
        }
    }
}
