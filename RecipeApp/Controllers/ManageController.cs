using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Models;
using RecipeApp.Models.BindingModels;
using RecipeApp.Models.BuissneModels;
using RecipeApp.Models.ViewModels;
using System.IO;
using RecipeApp.Repositories.Contracts;

namespace RecipeApp.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRecipeRepository recipeRepository;


        public ManageController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment webHostEnvironment, IRecipeRepository recipeRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.webHostEnvironment = webHostEnvironment;
            this.recipeRepository = recipeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details()
        {
            OwnerMock owner = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            return View(new AccountDetailsViewModel { Username = owner.Username, PhotoName = owner.Picture});
        }

        public IActionResult ViewProfile(string userId)
        {
            OwnerMock user = recipeRepository.FindOwnerById(userId);

            ViewProfileViewModel model = new ViewProfileViewModel()
            {
                User = user,
                UserRecipes = recipeRepository.FindRecipesByOwnerId(user.OwnerId).ToList(),
                OwnProfile = user.OwnerId == userManager.GetUserId(User)
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult EditDetails()
        {
            OwnerMock owner = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            return View(new AccountDetailsViewModel { Username = owner.Username, PhotoName = owner.Picture });
        }
       
        [HttpPost]
        public IActionResult EditDetails(AccountDetailsBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new AccountDetailsViewModel() { Image = model.Image, PhotoName = model.PhotoName, Username = model.Username });
            }

            OwnerMock owner = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            string uniqueFileName = UploadedFile(model);

            if (uniqueFileName == null)
            {
                owner.Username = model.Username;
            } 
            else
            {

                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "recipeImages");

                string oldPicture = owner.Picture;
                if (oldPicture != null)
                {
                    string filePath = Path.Combine(uploadsFolder, oldPicture);
                    System.IO.File.Delete(filePath);
                }

                owner.Username = model.Username;
                owner.Picture = uniqueFileName;
            }

            recipeRepository.UpdateOwner(owner);

            return RedirectToAction("Details", "Manage");
        }

        private string UploadedFile(AccountDetailsBindingModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "ownersImages");

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
