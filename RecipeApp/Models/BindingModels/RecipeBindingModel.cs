using Microsoft.AspNetCore.Http;
using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BindingModels
{
    public class RecipeBindingModel
    {
        public int RecipeId { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public TimeSpan TimeToCook { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [MaxLength(500)]
        public string Method { get; set; }

        public IEnumerable<Ingredient> Ingredients { get; set; }

        public IFormFile Image { get; set; }

        public string PhotoName { get; set; }

        public bool IsEditable { get; set; }

        public bool ReturnToHome { get; set; }

        public IEnumerable<Comment> Comments { get; set; }

        public string OwnerId { get; set; }

        public Comment Comment { get; set; }
        public ICollection<Step> Steps { get; set; } 
    }
}
