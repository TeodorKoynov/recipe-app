using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.ViewModels
{
    public class AddStepViewModel
    {
        public Step Step { get; set; }

        public int RecipeId { get; set; }
    }
}
