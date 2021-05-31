using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.ViewModels
{
    public class AccountDetailsViewModel
    {
        public string Username { get; set; }

        public IFormFile Image { get; set; }

        public string PhotoName { get; set; }
    }
}
