using Microsoft.AspNetCore.Identity;
using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models
{
    public class AppUser : IdentityUser
    {
        public List<Recipe> recipes;
    }
}
