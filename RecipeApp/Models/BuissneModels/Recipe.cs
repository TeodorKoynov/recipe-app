using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BuissneModels
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public TimeSpan TimeToCook { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        [MaxLength(500)]
        public string Method { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }

        public string Picture { get; set; }

        public OwnerMock Owner { get; set; }

        public string OwnerId { get; set; } 

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Step> Steps { get; set; }
    }
}
