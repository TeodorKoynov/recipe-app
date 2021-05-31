using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BindingModels
{
    public class HomeBindingModel
    {
        public IEnumerable<Recipe> recipes;
        public bool FromHome;
        public string SearchWord;
    }
}
