using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Recipe> recipes;
        public bool FromHome;
        public string SearchWord;
    }
}
