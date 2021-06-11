using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using RecipeApp.Models.BuissneModels;
using RecipeApp.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeDbContext context;

        private readonly IWebHostEnvironment webHostEnvironment;

        public RecipeRepository(RecipeDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public void AddIngredient(Ingredient ingredient, Recipe recipe)
        {
            recipe.Ingredients.Add(ingredient);
            context.SaveChanges();

        }

        public void AddRecipe(Recipe recipe)
        {
            context.Recipes.Add(recipe);
            context.SaveChanges();
        }

        public void DeleteRecipe(int id)
        {
            Recipe recipe = FindRecipeById(id);
            var ingredients = recipe.Ingredients;
            if (recipe != null)
            {
                if (recipe.Picture != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images", "recipeImages");

                    string FileName = recipe.Picture;

                    string filePath = Path.Combine(uploadsFolder, FileName);

                    File.Delete(filePath);
                }

                context.Recipes.Remove(recipe);
                context.Ingredients.RemoveRange(ingredients);
                context.SaveChanges();
            }
        }

        public Recipe FindRecipeById(int id)
        {
            Recipe recipe = context.Recipes.Include(r => r.Ingredients).Include(r => r.Comments).Include(r => r.Steps).FirstOrDefault(r => r.RecipeId == id);
            
            return recipe;
        }

        public IEnumerable<Recipe> GetAllRecipes()
        {
            return context.Recipes.ToList();
        }

        public void RemoveIngredient(int ingredientId, int recipeId)
        {
            Recipe recipe = context.Recipes.Find(recipeId);
            
            Ingredient ingredient = context.Ingredients.Find(ingredientId);

            recipe.Ingredients.Remove(ingredient);
            
            context.Ingredients.Remove(ingredient);
            
            context.SaveChanges();
        }

        public void UpdateRecipe(Recipe editedRecipe)
        {
            if(editedRecipe == null)
            {
                return;
            }

            context.Update(editedRecipe);

            context.SaveChanges();
        }

        public void UpdateIngredient(Ingredient editedIngredient)
        {
            if (editedIngredient == null)
            {
                return;
            }

            context.Update(editedIngredient);

            context.SaveChanges();
        }

        public IEnumerable<Recipe> FindRecipesByOwnerId(string ownerId)
        {
            List<Recipe> recipes = context.Recipes.Where(r => r.OwnerId == ownerId).ToList();

            return recipes;
        }

        public void AddOwner(OwnerMock owner)
        {
            context.Owners.Add(owner);
            context.SaveChanges();
        }

        public OwnerMock FindOwnerById(string ownerId)
        {
            OwnerMock owner = context.Owners.Include(o => o.Recipes).FirstOrDefault(o => o.OwnerId == ownerId);

            return owner;
        }

        public void UpdateOwner(OwnerMock owner)
        {
            if (owner == null)
            {
                return;
            }

            context.Owners.Update(owner);

            context.SaveChanges();
        }

        public void AddComment(Comment comment, Recipe recipe)
        {
            recipe.Comments.Add(comment);
            UpdateRecipe(recipe);

            context.SaveChanges();
        }

        public void UpdateComment(Comment comment, Recipe recipe)
        {
            if (comment == null)
            {
                return;
            }

            context.Update(comment);

            context.SaveChanges();
        }

        public void DeleteComent(Comment comment, Recipe recipe)
        {
            recipe.Comments.Remove(comment);

            context.Comments.Remove(comment);

            context.SaveChanges();
        }

        public Comment FindComment(int commentId)
        {
            Comment comment = context.Comments.Where(c => c.CommentId == commentId).FirstOrDefault();

            return comment;
        }

        public void AddStep(Step step, Recipe recipe)
        {
            recipe.Steps.Add(step);
            UpdateRecipe(recipe);

            context.SaveChanges(); 
        }

        public Step FindStepById(int stepId)
        {
            Step step = context.Steps.FirstOrDefault(s => s.StepId == stepId);

            return step;
        }

        public void UpdateStep(Step editedStep)
        {
            if (editedStep == null)
            {
                return;
            }

            context.Steps.Update(editedStep);

            context.SaveChanges();
        }

        public void DeleteStep(int recipeId, int stepId)
        {
            Recipe recipe = FindRecipeById(recipeId);
            Step step = FindStepById(stepId);

            recipe.Steps.Remove(step);

            context.Steps.Remove(step);

            context.SaveChanges();
        }
    }
}
