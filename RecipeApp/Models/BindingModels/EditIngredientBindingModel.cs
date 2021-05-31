using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BindingModels
{
    public class EditIngredientBindingModel
    {
        public Ingredient Ingredient { get; set; }

        public int RecipeId {get; set;}

        public int IngredientId { get; set; }
    }
}
