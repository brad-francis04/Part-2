namespace Part_2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace CommandLineApp.RecipeManager
    {
        class Ingredient //the gets and sets for ingreidents class
        {
            public string Name { get; set; }
            public double Quantity { get; set; }
            public string Unit { get; set; }
            public int Calories { get; set; }
            public string FoodGroup { get; set; }
        }

        class Recipe // gets and sets for recipe class
        {
            public string Name { get; set; }
            public List<Ingredient> Ingredients { get; set; }
            public List<string> Steps { get; set; }
            private double scaleFactor = 1.0;

            public Recipe() //the lists and steps of ingredients once users chooses to display a recipe
            {
                Ingredients = new List<Ingredient>();
                Steps = new List<string>();
            }

            public void GetRecipeDetails()
            {
                Console.Write("Enter the name of the recipe: "); //user must enter a title for their recipe
                Name = Console.ReadLine();

                Console.Write("Enter the number of ingredients: "); // once user inputs number of ingredients they will need to fill in the following
                int numIngredients = int.Parse(Console.ReadLine());

                for (int i = 0; i < numIngredients; i++)
                {
                    var ingredient = new Ingredient();
                    Console.WriteLine($"Ingredient {i + 1}:");
                    Console.Write("Name: ");
                    ingredient.Name = Console.ReadLine();
                    Console.Write("Quantity: ");
                    ingredient.Quantity = double.Parse(Console.ReadLine());
                    Console.Write("Unit: ");
                    ingredient.Unit = Console.ReadLine();
                    Console.Write("Calories: ");
                    ingredient.Calories = int.Parse(Console.ReadLine());
                    Console.Write("Food Group: ");
                    ingredient.FoodGroup = Console.ReadLine();
                    Ingredients.Add(ingredient);
                }

                Console.Write("Enter the number of steps: "); // user must enter number of steps in their recipe
                int numSteps = int.Parse(Console.ReadLine());

                for (int i = 0; i < numSteps; i++)
                {
                    Console.Write($"Step {i + 1}: ");
                    Steps.Add(Console.ReadLine());
                }
            }

            public void DisplayRecipe()
            {
                Console.WriteLine($"Recipe: {Name}");
                foreach (var ingredient in Ingredients)
                {
                    Console.WriteLine($"- {ingredient.Quantity * scaleFactor} {ingredient.Unit} of {ingredient.Name} ({ingredient.Calories} calories, {ingredient.FoodGroup})");
                }
                Console.WriteLine();
                for (int i = 0; i < Steps.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Steps[i]}");
                }
                Console.WriteLine($"Total Calories: {CalculateTotalCalories()}");
                Console.WriteLine();
            }

            public void ScaleRecipe(double factor)
            {
                scaleFactor = factor;
            }

            public void ResetScaling()
            {
                scaleFactor = 1.0;
            }

            public void ClearRecipe()
            {
                Ingredients.Clear();
                Steps.Clear();
                scaleFactor = 1.0;
            }

            public int CalculateTotalCalories()
            {
                return Ingredients.Sum(ingredient => ingredient.Calories);
            }
        }

        class RecipeManager
        {
            public List<Recipe> Recipes { get; set; }
            public delegate void CalorieNotificationHandler(string message);
            public event CalorieNotificationHandler CalorieNotification;

            public RecipeManager()
            {
                Recipes = new List<Recipe>();
            }

            public void AddRecipe(Recipe recipe)
            {
                Recipes.Add(recipe);
                CheckCalories(recipe);
            }

            public void DisplayAllRecipes()
            {
                Console.WriteLine("Recipes:");
                foreach (var recipe in Recipes.OrderBy(r => r.Name))
                {
                    Console.WriteLine(recipe.Name);
                }
                Console.WriteLine();
            }

            public Recipe GetRecipeByName(string name)
            {
                return Recipes.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            private void CheckCalories(Recipe recipe)
            {
                if (recipe.CalculateTotalCalories() > 300)
                {
                    CalorieNotification?.Invoke($"Warning: The total calories of {recipe.Name} exceed 300!");// if the users calories exceed 300 they will recieve this message
                }
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                RecipeManager manager = new RecipeManager();
                manager.CalorieNotification += (message) => Console.WriteLine(message);// the below is the interface the user is greeted with

                while (true)
                {
                    Console.WriteLine("Welcome to the Recipe Manager!");
                    Console.WriteLine("1. Enter a new recipe");
                    Console.WriteLine("2. Display all recipes");
                    Console.WriteLine("3. Display a recipe by name");
                    Console.WriteLine("4. Scale a recipe");
                    Console.WriteLine("5. Reset recipe scaling");
                    Console.WriteLine("6. Clear a recipe");
                    Console.WriteLine("7. Exit");

                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            Recipe recipe = new Recipe();
                            recipe.GetRecipeDetails();
                            manager.AddRecipe(recipe);
                            break;
                        case 2:
                            manager.DisplayAllRecipes();
                            break;
                        case 3:
                            Console.Write("Enter the name of the recipe to display: ");
                            string name = Console.ReadLine();
                            Recipe foundRecipe = manager.GetRecipeByName(name);
                            if (foundRecipe != null)
                            {
                                foundRecipe.DisplayRecipe();
                            }
                            else
                            {
                                Console.WriteLine("Recipe not found.");
                            }
                            break;
                        case 4:
                            Console.Write("Enter the name of the recipe to scale: ");
                            string scaleName = Console.ReadLine();
                            Recipe scaleRecipe = manager.GetRecipeByName(scaleName);
                            if (scaleRecipe != null)
                            {
                                Console.Write("Enter the scaling factor (0.5, 2, or 3): ");
                                double factor;
                                if (double.TryParse(Console.ReadLine(), out factor))
                                {
                                    scaleRecipe.ScaleRecipe(factor);
                                    Console.WriteLine("Recipe scaled.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid scaling factor.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Recipe not found.");
                            }
                            break;
                        case 5:
                            Console.Write("Enter the name of the recipe to reset scaling: ");
                            string resetName = Console.ReadLine();
                            Recipe resetRecipe = manager.GetRecipeByName(resetName);
                            if (resetRecipe != null)
                            {
                                resetRecipe.ResetScaling();
                                Console.WriteLine("Scaling has been reset to 1.0.");
                            }
                            else
                            {
                                Console.WriteLine("Recipe not found.");
                            }
                            break;
                        case 6:
                            Console.Write("Enter the name of the recipe to clear: ");
                            string clearName = Console.ReadLine();
                            Recipe clearRecipe = manager.GetRecipeByName(clearName);
                            if (clearRecipe != null)
                            {
                                clearRecipe.ClearRecipe();
                                manager.Recipes.Remove(clearRecipe);
                                Console.WriteLine("Recipe has been cleared.");
                            }
                            else
                            {
                                Console.WriteLine("Recipe not found.");
                            }
                            break;
                        case 7:
                            Console.WriteLine("Exiting the Recipe Manager...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
