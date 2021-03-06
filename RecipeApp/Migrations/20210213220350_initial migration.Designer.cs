// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeApp.Models;

namespace RecipeApp.Migrations
{
    [DbContext(typeof(RecipeDbContext))]
    [Migration("20210213220350_initial migration")]
    partial class initialmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RecipeApp.Models.BuissneModels.Ingredient", b =>
                {
                    b.Property<int>("IngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("varchar(40) CHARACTER SET utf8mb4")
                        .HasMaxLength(40);

                    b.Property<decimal>("Quantity")
                        .HasColumnName("quantity")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int>("RecipeId")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnName("unit")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("IngredientId")
                        .HasName("ingredients_id");

                    b.HasIndex("RecipeId");

                    b.ToTable("ingredients");
                });

            modelBuilder.Entity("RecipeApp.Models.BuissneModels.Recipe", b =>
                {
                    b.Property<int>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnName("is_deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnName("method")
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<TimeSpan>("TimeToCook")
                        .HasColumnName("time_to_cook")
                        .HasColumnType("time(6)");

                    b.HasKey("RecipeId")
                        .HasName("recipes_id");

                    b.ToTable("recipes");
                });

            modelBuilder.Entity("RecipeApp.Models.BuissneModels.Ingredient", b =>
                {
                    b.HasOne("RecipeApp.Models.BuissneModels.Recipe", "Recipe")
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
