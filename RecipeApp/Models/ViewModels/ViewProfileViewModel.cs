using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.ViewModels
{
    public class ViewProfileViewModel
    {
        public OwnerMock User { get; set; }

        public List<Recipe> UserRecipes { get; set; }

        public bool OwnProfile { get; set; }
    }
}
