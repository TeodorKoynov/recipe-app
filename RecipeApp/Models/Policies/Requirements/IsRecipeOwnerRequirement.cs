using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.Policies.Requirements
{
    public class IsRecipeOwnerRequirement: IAuthorizationRequirement
    {
    }
}
