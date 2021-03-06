using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Models;
using RecipeApp.Models.BindingModels;
using RecipeApp.Models.BuissneModels;
using RecipeApp.Models.ViewModels;
using RecipeApp.Repositories.Contracts;

namespace RecipeApp.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository recipeRepository;

        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly UserManager<AppUser> userManager;

        private IAuthorizationService authorizationService;

        public RecipeController(IRecipeRepository recipeRepository, IWebHostEnvironment webHostEnvironment,
            UserManager<AppUser> userManager, IAuthorizationService authorizationService)
        {
            this.recipeRepository = recipeRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (userManager.GetUserId(User) != null && (recipeRepository.FindOwnerById(userManager.GetUserId(User)) == null))
            {
                OwnerMock owner = new OwnerMock()
                {
                    OwnerId = userManager.GetUserId(User),
                    Picture = null,
                    Recipes = new List<Recipe>(),
                    Username = "User"
                };

                recipeRepository.AddOwner(owner);

            }
            List<Recipe> recipes = recipeRepository.FindRecipesByOwnerId(userManager.GetUserId(User)).ToList();

            return View(recipes);
        }

        [HttpGet]
        public async Task<IActionResult> View(int recipeId)
        {
            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            if (recipe == null)
            {
                return View("Error", new ErrorViewModel() { RequestId = "404" });
            }

            UpdateComments(recipe);

            var authResult = await authorizationService.AuthorizeAsync(User, recipe, "CanManageReicpe");

            RecipeViewModel model = new RecipeViewModel
            {
                PhotoName = recipe.Picture,
                Ingredients = recipe.Ingredients,
                Method = recipe.Method,
                RecipeId = recipe.RecipeId,
                Name = recipe.Name,
                TimeToCook = recipe.TimeToCook,
                IsDeleted = recipe.IsDeleted,
                IsEditable = authResult.Succeeded,
                ReturnToHome = true,
                Comments = recipe.Comments,
                OwnerId = recipe.OwnerId,
                Steps = recipe.Steps,
                CurrentUser = userManager.GetUserId(User)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> View(RecipeBindingModel bindingModel, int recipeId)
        {
            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            if (recipe == null)
            {
                return View("Error", new ErrorViewModel() { RequestId = "404" });
            }

            var authResult = await authorizationService.AuthorizeAsync(User, recipe, "CanManageReicpe");

            if (bindingModel.Comment.Message == null)
            {
                return View(new RecipeViewModel
                {
                    Name = recipe.Name,
                    Ingredients = recipe.Ingredients,
                    IsEditable = authResult.Succeeded,
                    RecipeId = recipe.RecipeId,
                    ReturnToHome = true,
                    IsDeleted = recipe.IsDeleted,
                    Method = recipe.Method,
                    PhotoName = recipe.Picture,
                    TimeToCook = recipe.TimeToCook,
                    Comments = recipe.Comments,
                    OwnerId = recipe.OwnerId,
                    Steps = recipe.Steps
                });
            }

            Comment comment = new Comment()
            {
                Message = bindingModel.Comment.Message,
                Recipe = recipe,
                RecipeId = recipeId,
                WriterPhoto = recipeRepository.FindOwnerById(userManager.GetUserId(User)).Picture
            };

            UpdateComments(recipe);

            comment.WriterId = userManager.GetUserId(User);

            comment.WriterUsername = recipeRepository.FindOwnerById(comment.WriterId).Username;

            recipeRepository.AddComment(comment, recipe);

            RecipeViewModel model = new RecipeViewModel
            {
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsEditable = authResult.Succeeded,
                RecipeId = recipe.RecipeId,
                ReturnToHome = true,
                IsDeleted = recipe.IsDeleted,
                Method = recipe.Method,
                PhotoName = recipe.Picture,
                TimeToCook = recipe.TimeToCook,
                Comments = recipe.Comments,
                Comment = new Comment
                {
                    Message = ""
                },
                OwnerId = recipe.OwnerId
            };

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            recipeRepository.DeleteRecipe(id);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditRecipe(int id)
        {
            Recipe recipeToEdit = recipeRepository.FindRecipeById(id);

            var authResult = await authorizationService.AuthorizeAsync(User, recipeToEdit, "CanManageReicpe");

            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }

            RecipeViewModel model = new RecipeViewModel
            {
                RecipeId = recipeToEdit.RecipeId,
                Name = recipeToEdit.Name,
                Ingredients = recipeToEdit.Ingredients,
                Method = recipeToEdit.Method,
                TimeToCook = recipeToEdit.TimeToCook,
                PhotoName = recipeToEdit.Picture,
                OwnerId = recipeToEdit.OwnerId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditRecipe(RecipeBindingModel bindingModel, int recipeId)
        {
            if (!ModelState.IsValid)
            {
                return View(new RecipeViewModel { Name = bindingModel.Name, Method = bindingModel.Method, TimeToCook = bindingModel.TimeToCook, RecipeId = bindingModel.RecipeId, Image = bindingModel.Image, PhotoName = bindingModel.PhotoName });
            } 

            Recipe currentRecipe = recipeRepository.FindRecipeById(recipeId);

            currentRecipe.Name = bindingModel.Name;
            currentRecipe.Method = bindingModel.Method;
            currentRecipe.TimeToCook = bindingModel.TimeToCook;
            if (bindingModel.Image != null)
            {
                string uniqueFileName = UploadedFile(bindingModel);
                currentRecipe.Picture = uniqueFileName;
            };

            recipeRepository.UpdateRecipe(currentRecipe);

            return RedirectToAction("View", new { id = currentRecipe.RecipeId });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RecipeViewModel { });
        }

        [HttpPost]
        public IActionResult Create(RecipeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new RecipeViewModel { RecipeId = model.RecipeId, Ingredients = model.Ingredients, IsDeleted = model.IsDeleted, Method = model.Method, Name = model.Name, TimeToCook = model.TimeToCook, Image = model.Image });
            }

            string uniqueFileName = UploadedFile(model);

            Recipe recipe = new Recipe()
            {
                Name = model.Name,
                Ingredients = new List<Ingredient>(),
                Method = model.Method,
                TimeToCook = model.TimeToCook,
                IsDeleted = false,
                Picture = uniqueFileName,
                OwnerId = userManager.GetUserId(User),
                Comments = new List<Comment>(),
                Owner = recipeRepository.FindOwnerById(userManager.GetUserId(User)),
                Steps = new List<Step>()
            };

            OwnerMock owner = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            recipeRepository.AddRecipe(recipe);

            owner.Recipes.Add(recipe);

            recipeRepository.UpdateOwner(owner);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddStep(int recipeId)
        {
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();
            if (recipe != null)
            {
                AddStepViewModel viewModel = new AddStepViewModel()
                {
                    Step = new Step { },
                    RecipeId = recipeId
                };

                return View(viewModel);
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpPost]
        public IActionResult AddStep(AddStepViewModel bindingModel)
        {
            if (!ModelState.IsValid)
            {
                return View(new AddStepViewModel() { RecipeId = bindingModel.RecipeId, Step = new Step() { Instructions = bindingModel.Step.Instructions, RecipeId = bindingModel.RecipeId } });
            }

            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == bindingModel.RecipeId)
                .FirstOrDefault();
            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                recipeRepository.AddStep(bindingModel.Step, currentRecipe);
                return RedirectToAction("View", new { id = bindingModel.RecipeId });
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        public IActionResult DeleteStep(int stepId, int recipeId)
        {
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();
            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                Step currentStep = currentRecipe.Steps.Where(s => s.StepId == stepId)
                .FirstOrDefault();

                if (currentStep != null)
                {
                    recipeRepository.DeleteStep(recipeId, stepId);

                    return RedirectToAction("View", new { id = recipeId });
                }
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpGet]
        public IActionResult EditStep(int recipeId, int stepId)
        {
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();
            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                Step currentStep = currentRecipe.Steps.Where(s => s.StepId == stepId)
                .FirstOrDefault();

                if (currentStep != null)
                {
                    EditStepViewModel model = new EditStepViewModel { Step = currentStep, StepId = stepId, RecipeId = recipeId };

                    return View(model);
                }
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpPost]
        public IActionResult EditStep(Step step, int recipeId, int stepId)
        {
            if (!ModelState.IsValid)
            {
                return View(new EditStepViewModel { Step = step, RecipeId = recipeId, StepId = stepId });
            }

            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();

            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                Step currentStep = currentRecipe.Steps.Where(s => s.StepId == stepId)
                .FirstOrDefault();

                if (currentStep != null)
                {
                    currentStep.Instructions = step.Instructions;

                    recipeRepository.UpdateStep(currentStep);

                    return RedirectToAction("View", new { id = recipeId });
                }
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpGet]
        public IActionResult AddIngredient(int recipeId)
        {
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();

            if (recipe != null)
            {
                AddIngredientViewModel viewModel = new AddIngredientViewModel
                {
                    Ingredient = new Ingredient { },
                    RecipeId = recipeId
                };
                return View(viewModel);
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpPost]
        public IActionResult AddIngredient(AddIngredientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new AddIngredientViewModel { Ingredient = new Ingredient() { Name = model.Ingredient.Name, Quantity = model.Ingredient.Quantity, Unit = model.Ingredient.Unit }, RecipeId = model.RecipeId });
            }

            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == model.RecipeId)
                .FirstOrDefault();

            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);
                
                recipeRepository.AddIngredient(model.Ingredient, currentRecipe);

                return RedirectToAction("View", new { id = model.RecipeId });

            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        public IActionResult DeleteIngredient(int ingredientId, int recipeId)
        {
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();

            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                Ingredient currentIngredient = currentRecipe.Ingredients.Where(i => i.IngredientId == ingredientId).FirstOrDefault();

                if (currentIngredient != null)
                {
                    recipeRepository.RemoveIngredient(ingredientId, recipeId);

                    return RedirectToAction("View", new { id = recipeId });
                }

            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpGet]
        public IActionResult EditIngredient(int recipeId, int ingredientId)
        {
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();

            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                Ingredient currentIngredient = currentRecipe.Ingredients.Where(i => i.IngredientId == ingredientId).FirstOrDefault();

                if (currentIngredient != null)
                {
                    EditIngredientViewModel model = new EditIngredientViewModel { Ingredient = currentIngredient, IngredientId = ingredientId, RecipeId = recipeId };

                    return View(model);
                }
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpPost]
        public IActionResult EditIngredient(Ingredient ingredient, int recipeId, int ingredientId)
        {
            if (!ModelState.IsValid)
            {
                return View(new EditIngredientViewModel { Ingredient = ingredient, IngredientId = ingredientId, RecipeId = recipeId });
            }
            
            OwnerMock currentUser = recipeRepository.FindOwnerById(userManager.GetUserId(User));

            Recipe recipe = currentUser.Recipes
                .Where(r => r.RecipeId == recipeId)
                .FirstOrDefault();

            if (recipe != null)
            {
                Recipe currentRecipe = recipeRepository.FindRecipeById(recipe.RecipeId);

                Ingredient currentIngredient = currentRecipe.Ingredients.Where(i => i.IngredientId == ingredientId).FirstOrDefault();

                if (currentIngredient != null)
                {
                    currentIngredient.Name = ingredient.Name;
                    currentIngredient.Quantity = ingredient.Quantity;
                    currentIngredient.Unit = ingredient.Unit;

                    recipeRepository.UpdateIngredient(currentIngredient);

                    return RedirectToAction("View", new { id = recipeId });
                }
            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        [HttpGet]
        public IActionResult Home()
        {
            List<Recipe> recipes = recipeRepository.GetAllRecipes().ToList();

            HomeViewModel model = new HomeViewModel
            {
                recipes = recipes,
                FromHome = true,
                SearchWord = ""
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Home(string searchWord)
        {
            List<Recipe> recipes = recipeRepository.GetAllRecipes().ToList();

            string keyWord;

            if (searchWord == null)
            {
                keyWord = "";
            }
            else
            {
                keyWord = searchWord.TrimStart().TrimEnd().ToLower(); ;
            }

            List<Recipe> filteredRecipes = new List<Recipe>();

            foreach (Recipe recipe in recipes)
            {
                if (recipe.Name.ToLower().TrimEnd().ToLower().Contains(keyWord))
                {
                    filteredRecipes.Add(recipe);
                }
            }

            HomeViewModel model = new HomeViewModel()
            {
                FromHome = true,
                recipes = filteredRecipes,
                SearchWord = keyWord
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewFromHome(int recipeId)
        {
            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            if (recipe == null)
            {
                return View("Error", new ErrorViewModel() { RequestId = "404" });
            }

            UpdateComments(recipe);

            var authResult = await authorizationService.AuthorizeAsync(User, recipe, "CanManageReicpe");

            RecipeViewModel model = new RecipeViewModel
            {
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsEditable = authResult.Succeeded,
                RecipeId = recipe.RecipeId,
                ReturnToHome = true,
                IsDeleted = recipe.IsDeleted,
                Method = recipe.Method,
                PhotoName = recipe.Picture,
                TimeToCook = recipe.TimeToCook,
                Comments = recipe.Comments,
                Steps = recipe.Steps,
                OwnerId = recipe.OwnerId,
                CurrentUser = userManager.GetUserId(User)
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ViewFromHome(RecipeBindingModel bindingModel, int recipeId)
        {
            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            if (recipe == null)
            {
                return View("Error", new ErrorViewModel() { RequestId = "404" });
            }

            var authResult = await authorizationService.AuthorizeAsync(User, recipe, "CanManageReicpe");

            if (bindingModel.Comment.Message == null)
            {
                return View(new RecipeViewModel
                {
                    Name = recipe.Name,
                    Ingredients = recipe.Ingredients,
                    IsEditable = authResult.Succeeded,
                    RecipeId = recipe.RecipeId,
                    ReturnToHome = true,
                    IsDeleted = recipe.IsDeleted,
                    Method = recipe.Method,
                    PhotoName = recipe.Picture,
                    TimeToCook = recipe.TimeToCook,
                    Comments = recipe.Comments,
                    Steps = recipe.Steps,
                    CurrentUser = userManager.GetUserId(User)
                });
            }

            Comment comment = new Comment()
            {
                Message = bindingModel.Comment.Message,
                Recipe = recipe,
                RecipeId = recipeId,
                WriterPhoto = recipeRepository.FindOwnerById(userManager.GetUserId(User)).Picture
            };

            UpdateComments(recipe);

            comment.WriterId = userManager.GetUserId(User);

            comment.WriterUsername = recipeRepository.FindOwnerById(comment.WriterId).Username;

            recipeRepository.AddComment(comment, recipe);


            RecipeViewModel model = new RecipeViewModel
            {
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsEditable = authResult.Succeeded,
                RecipeId = recipe.RecipeId,
                ReturnToHome = true,
                IsDeleted = recipe.IsDeleted,
                Method = recipe.Method,
                PhotoName = recipe.Picture,
                TimeToCook = recipe.TimeToCook,
                Comments = recipe.Comments,
                Comment = new Comment
                {
                    Message = ""
                },
                OwnerId = recipe.OwnerId,
                Steps = recipe.Steps,
                CurrentUser = userManager.GetUserId(User)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> ViewFromProfile(int recipeId)
        {
            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            if (recipe == null)
            {
                return View("Error", new ErrorViewModel() { RequestId = "404" });
            }

            UpdateComments(recipe);

            var authResult = await authorizationService.AuthorizeAsync(User, recipe, "CanManageReicpe");

            RecipeViewModel model = new RecipeViewModel
            {
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsEditable = authResult.Succeeded,
                RecipeId = recipe.RecipeId,
                IsDeleted = recipe.IsDeleted,
                Method = recipe.Method,
                PhotoName = recipe.Picture,
                TimeToCook = recipe.TimeToCook,
                Comments = recipe.Comments,
                OwnerId = recipe.OwnerId,
                Steps = recipe.Steps,
                CurrentUser = userManager.GetUserId(User)
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ViewFromProfile(RecipeBindingModel bindingModel, int recipeId)
        {
            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            var authResult = await authorizationService.AuthorizeAsync(User, recipe, "CanManageReicpe");

            if (bindingModel.Comment.Message == null)
            {
                return View(new RecipeViewModel
                {
                    Name = recipe.Name,
                    Ingredients = recipe.Ingredients,
                    IsEditable = authResult.Succeeded,
                    RecipeId = recipe.RecipeId,
                    ReturnToHome = true,
                    IsDeleted = recipe.IsDeleted,
                    Method = recipe.Method,
                    PhotoName = recipe.Picture,
                    TimeToCook = recipe.TimeToCook,
                    Comments = recipe.Comments,
                    OwnerId = recipe.OwnerId,
                    Steps = recipe.Steps,
                    CurrentUser = userManager.GetUserId(User)
                });
            }

            Comment comment = new Comment()
            {
                Message = bindingModel.Comment.Message,
                Recipe = recipe,
                RecipeId = recipeId,
                WriterPhoto = recipeRepository.FindOwnerById(userManager.GetUserId(User)).Picture
            };

            UpdateComments(recipe);

            comment.WriterId = userManager.GetUserId(User);

            comment.WriterUsername = recipeRepository.FindOwnerById(comment.WriterId).Username;

            recipeRepository.AddComment(comment, recipe);

            RecipeViewModel model = new RecipeViewModel
            {
                Name = recipe.Name,
                Ingredients = recipe.Ingredients,
                IsEditable = authResult.Succeeded,
                RecipeId = recipe.RecipeId,
                ReturnToHome = true,
                IsDeleted = recipe.IsDeleted,
                Method = recipe.Method,
                PhotoName = recipe.Picture,
                TimeToCook = recipe.TimeToCook,
                Comments = recipe.Comments,
                Comment = new Comment
                {
                    Message = ""
                },
                OwnerId = recipe.OwnerId,
                Steps = recipe.Steps,
                CurrentUser = userManager.GetUserId(User)
            };

            return RedirectToAction("ViewFromProfile", new { recipeId = model.RecipeId });
        }

        public IActionResult DeleteComment(int commentId, int recipeId, string redirect)
        {

            Comment comment = recipeRepository.FindComment(commentId);

            Recipe recipe = recipeRepository.FindRecipeById(recipeId);

            if (comment.WriterId == userManager.GetUserId(User))
            {
                recipeRepository.DeleteComent(comment, recipe);
                return RedirectToAction(redirect, new { recipeId = recipeId});

            }

            return View("Error", new ErrorViewModel() { RequestId = "404" });
        }

        private void UpdateComments(Recipe recipe)
        {
            if(recipe != null)
            {
                foreach (Comment oldComment in recipe.Comments)
                {
                    oldComment.WriterPhoto = recipeRepository.FindOwnerById(oldComment.WriterId).Picture;
                    oldComment.WriterUsername = recipeRepository.FindOwnerById(oldComment.WriterId).Username;
                }
            } 
            else
            {
                return;
            }
        }

        private string UploadedFile(RecipeBindingModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "recipeImages");

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
