using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Models.BuissneModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models
{
    public class RecipeDbContext : IdentityDbContext<AppUser>
    {
        public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
            : base(options)
        {

        }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<OwnerMock> Owners { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Step> Steps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var recipe = builder.Entity<Recipe>();

            recipe.ToTable("recipes");

            recipe.HasKey(r => r.RecipeId)
                .HasName("recipes_id");

            recipe.Property(r => r.RecipeId)
                .HasColumnName("id");

            recipe.Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("name");

            recipe.Property(r => r.TimeToCook)
                .IsRequired()
                .HasColumnName("time_to_cook");

            recipe.Property(r => r.IsDeleted)
                .IsRequired()
                .HasColumnName("is_deleted");

            recipe.Property(r => r.Method)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("method");

            recipe.Property(r => r.Picture)
                .HasColumnName("picture");

            recipe.HasMany(r => r.Ingredients)
                .WithOne(i => i.Recipe)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            recipe.HasMany(r => r.Comments)
                .WithOne(c => c.Recipe)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            recipe.HasMany(r => r.Steps)
                .WithOne(s => s.Recipe)
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);



            var ingredient = builder.Entity<Ingredient>();

            ingredient.ToTable("ingredients");

            ingredient.HasKey(i => i.IngredientId)
                .HasName("ingredients_id");

            ingredient.Property(i => i.IngredientId)
                .HasColumnName("id");

            ingredient.Property(i => i.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(40);

            ingredient.Property(i => i.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            ingredient.Property(i => i.Unit)
                .IsRequired()
                .HasColumnName("unit");



            var owners = builder.Entity<OwnerMock>();

            owners.ToTable("user_mock");

            owners.HasKey(u => u.OwnerId)
                .HasName("users_id");

            owners.Property(u => u.OwnerId)
                .HasColumnName("id");

            owners.Property(u => u.Picture)
                .HasColumnName("photo");

            owners.HasMany(u => u.Recipes)
                .WithOne(r => r.Owner)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);



            var comment = builder.Entity<Comment>();

            comment.ToTable("recipe_comments");

            comment.HasKey(c => c.CommentId)
                .HasName("comment_id");

            comment.Property(c => c.CommentId)
                .HasColumnName("id");

            comment.Property(c => c.Message)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("message");

            comment.Property(c => c.WriterId)
                .HasColumnName("writer_id")
                .IsRequired();

            comment.Property(c => c.WriterUsername)
                .HasColumnName("writer_username");

            comment.Property(c => c.WriterPhoto)
                .HasColumnName("writer_photo");


            var step = builder.Entity<Step>();

            step.ToTable("recipe_steps");

            step.HasKey(s => s.StepId)
                .HasName("step_id");

            step.Property(s => s.StepId)
                .HasColumnName("id");

            step.Property(s => s.Instructions)
                .HasMaxLength(300)
                .IsRequired()
                .HasColumnName("instructions");



            base.OnModelCreating(builder);
        }
    }
}
