using RecipeApp.Models.BuissneModels;
using RecipeApp.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Repositories
{
    public class MockedRecipeRepository : IRecipeRepository
    {
        private List<Recipe> recipes = new List<Recipe>()
        {
            new Recipe()
            {
                RecipeId = 0,
                Name = "Mango with ice",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient()
                    {
                        IngredientId = 1,
                        Name = "Mango",
                        RecipeId = 1,
                        Quantity = 1,
                        Unit = "1kg"
                    },
                    new Ingredient()
                    {
                        IngredientId = 2,
                        Name = "Ice",
                        RecipeId = 1,
                        Quantity = 1,
                        Unit = "0.5kg"
                    }
                },
                IsDeleted = false,
                Method = "Chop the mango and add ice",
                TimeToCook = new TimeSpan(1, 0, 0)
            },
            new Recipe()
            {
                RecipeId = 1,
                Name = "Coco with honey",
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient()
                    {
                        IngredientId = 3,
                        Name = "Coco",
                        RecipeId = 2,
                        Quantity = 1,
                        Unit = "1kg"
                    },
                    new Ingredient()
                    {
                        IngredientId = 4,
                        Name = "Honey",
                        RecipeId = 2,
                        Quantity = 1,
                        Unit = "0.5kg"
                    },
                },
                                IsDeleted = false,
                Method = "Chop the coco and add honey",
                TimeToCook = new TimeSpan(1, 0, 0)
            }
        };

        private int nextRecipeId = 3;
        private int nextIngredientId = 5;

        public void AddRecipe(Recipe recipe)
        {
            recipe.RecipeId = nextRecipeId;
            nextRecipeId++;
            recipes.Add(recipe);
        }

        public void DeleteRecipe(int id)
        {
            recipes.Remove(recipes.FirstOrDefault(r => r.RecipeId == id));
        }

        public Recipe FindRecipeById(int id)
        {
            Recipe recipe = recipes.FirstOrDefault(r => r.RecipeId == id);
            return recipe;
        }

        public IEnumerable<Recipe> GetAllRecipes()
        {
            return recipes;
        }

        public void UpdateRecipe(Recipe editedRecipe)
        {
            Recipe currentRecipe = recipes.FirstOrDefault(r => r.RecipeId == editedRecipe.RecipeId);

            currentRecipe.Name = editedRecipe.Name;
            currentRecipe.Method = editedRecipe.Method;
            currentRecipe.TimeToCook = editedRecipe.TimeToCook;
        }

        public void AddIngredient(Ingredient ingredient, Recipe recipe)
        {
            ingredient.IngredientId = nextIngredientId;
            ingredient.RecipeId = recipe.RecipeId;
            recipe.Ingredients.Add(ingredient);
            nextIngredientId++;
        }

        public void RemoveIngredient(int ingredientId, int recipeId)
        {
            foreach (Recipe currentRecipe in recipes)
            {
                if (currentRecipe.RecipeId == recipeId)
                {
                    Ingredient ingredientToRemove = currentRecipe.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);

                    currentRecipe.Ingredients.Remove(ingredientToRemove);

                    return;
                }
            }
        }

        public void UpdateIngredient(Ingredient editedIngredient)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Recipe> FindRecipesByOwnerId(string ownerId)
        {
            throw new NotImplementedException();
        }

        public void AddOwner(OwnerMock owner)
        {
            throw new NotImplementedException();
        }

        public void UpdateOwner(OwnerMock owner)
        {
            throw new NotImplementedException();
        }

        public OwnerMock FindOwnerById(string ownerId)
        {
            throw new NotImplementedException();
        }

        public void AddComment(Comment comment, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public void UpdateComment(Comment comment, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public void DeleteComent(Comment comment, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Comment FindComment(int commentId)
        {
            throw new NotImplementedException();
        }

        public void AddStep(Step step, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Step FindStepById(int stepId)
        {
            throw new NotImplementedException();
        }

        public void UpdateStep(Step editedStep)
        {
            throw new NotImplementedException();
        }

        public void DeleteStep(int recipeId, int stepId)
        {
            throw new NotImplementedException();
        }
    }
}
