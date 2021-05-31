using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BuissneModels
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        [Required]
        [MaxLength(40)]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string Unit { get; set; }

        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }
    }
}
