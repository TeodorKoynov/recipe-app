using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RecipeApp.Models.BuissneModels;
using RecipeApp.Models.Policies.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.Policies.Handlers
{
    public class IsRecipeOwnerHandler : 
        AuthorizationHandler<IsRecipeOwnerRequirement, Recipe>
    {
        private readonly UserManager<AppUser> userManager;

        public IsRecipeOwnerHandler(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            IsRecipeOwnerRequirement requirement, 
            Recipe resource)
        {
            var appUser = await userManager.GetUserAsync(context.User);
            if (appUser == null)
            {
                return;
            }

            if(resource.OwnerId == appUser.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}
