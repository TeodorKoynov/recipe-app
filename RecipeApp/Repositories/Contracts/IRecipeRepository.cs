using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Repositories.Contracts
{
    public interface IRecipeRepository
    {
        public void AddRecipe(Recipe recipe);

        public void DeleteRecipe(int id);

        public Recipe FindRecipeById(int id);

        public IEnumerable<Recipe> GetAllRecipes();

        public void UpdateRecipe(Recipe editedRecipe);

        public void AddIngredient(Ingredient ingredient, Recipe recipe);

        public void RemoveIngredient(int ingredientId, int recipeId);

        public void UpdateIngredient(Ingredient editedIngredient);

        public IEnumerable<Recipe> FindRecipesByOwnerId(string ownerId);

        public void AddOwner(OwnerMock owner);

        public void UpdateOwner(OwnerMock owner);

        public OwnerMock FindOwnerById(string ownerId);

        public void AddComment(Comment comment, Recipe recipe);

        public void UpdateComment(Comment comment, Recipe recipe);

        public void DeleteComent(Comment comment, Recipe recipe);

        public Comment FindComment(int commentId);

        public void AddStep(Step step, Recipe recipe);

        public Step FindStepById(int stepId);

        public void UpdateStep(Step editedStep);

        public void DeleteStep(int recipeId, int stepId);

    }
}
