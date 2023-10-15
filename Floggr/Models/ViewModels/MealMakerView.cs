using RootFoundationFoods;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Floggr.Code;
using Microsoft.AspNetCore.Mvc;

namespace Floggr.Models.ViewModels
{
	public class MealMakerView
	{
		public SelectFood Protein { get; set; }
		public SelectFood Grain { get; set; }
		public SelectFood DairyFat { get; set; }
		public List<SelectFood> Fruits { get; set; }
		public List<SelectFood> Vegetables { get; set; }
		//public PaginatedList<SelectFood> AllFoundationFoods { get; set; }
		public List<SelectFood> AllFoundationFoods { get; set; }
		public string RecipeName { get; set; }
		public string RecipeIngredients { get; set; }
		public string RecipeInstructions { get; set; }

		/*
        public List<listFoods> foods { get; set; }
        public MealMakerView() { }
		public int proteinCount { get; set; }
		public SelectFood selectFood { get; set; }*/
		//public bool HasPreviousPage { get; set; }
  //      public bool HasNextPage { get; set; }
		//public int PageIndex { get; set; }


    }
	//public class listFoods
	//{
	//	public string foodName { get; set; }
	//	public string foodCat { get; set; }
	//}
	public class SelectFood
	{
		public string foodName { get; set; }
		public string foodCat { get; set; }
		public int foodID { get; set; }
	}
}
