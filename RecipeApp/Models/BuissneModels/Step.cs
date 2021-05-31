using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BuissneModels
{
    public class Step
    {
        public int StepId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Instructions { get; set; }

        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }
    }
}
