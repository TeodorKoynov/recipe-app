using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BindingModels
{
    public class AccountDetailsBindingModel
    {
        [Required]
        public string Username { get; set; }

        public IFormFile Image { get; set; }

        public string PhotoName { get; set; }
    }
}
