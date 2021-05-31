using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BuissneModels
{
    public class OwnerMock
    {
        public string OwnerId { get; set; }

        public string Picture { get; set; }

        public string Username { get; set; }

        public ICollection<Recipe> Recipes { get; set; }
    }
}
