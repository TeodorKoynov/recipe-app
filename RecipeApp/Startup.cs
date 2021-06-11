using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeApp.Models;
using RecipeApp.Repositories;
using RecipeApp.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RecipeApp.Models.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using RecipeApp.Models.Policies.Handlers;

namespace RecipeApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddDbContext<RecipeDbContext>(options => options.UseMySql("Server=127.0.0.1;Database=recipe_db;Uid=root;Pwd=root"));
            services.AddIdentity<AppUser, IdentityRole>(identity => 
                {
                    identity.Password.RequireDigit = false;
                    identity.Password.RequireNonAlphanumeric = false;
                    identity.Password.RequireUppercase = false;
                    identity.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<RecipeDbContext>().AddDefaultTokenProviders();
            services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            services.AddAuthorization(options => {
                options.AddPolicy("CanManageReicpe", policy => 
                policy.AddRequirements(new IsRecipeOwnerRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, IsRecipeOwnerHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Recipe}/{action=Home}/{id?}");
            });
        }
    }
}
