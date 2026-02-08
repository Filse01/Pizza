using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pizza.Models;

namespace Pizza.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedIngredientAsync(context);
        await SeedPizzaAsync(context);
        await SeedCouponsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
    {
        const string email = "admin@admin.com";
        const string password = "Test1234@";
        const string firstName = "Admin";
        const string lastName = "Admin";
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var adminUser = new IdentityUser()
            {
                Id = "6ee1b162-422b-4cb5-a476-1432346b7503",
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task SeedIngredientAsync(PizzaDbContext dbContext)
    {
        var path = Path.Combine("..","Pizza","Data","Ingredients.json");
        string ingredientJson = File.ReadAllText(path);
        try
        {
            var ingredients = JsonSerializer.Deserialize<List<Ingredient>>(ingredientJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() } 
                });
            if (ingredients != null && ingredients.Count > 0)
            {
                foreach (var ing in ingredients)
                {
                    if (await dbContext.Ingredients.AnyAsync(e => ing.Name == e.Name) == false)
                    {
                        await dbContext.Ingredients.AddAsync(ing);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization failed: {ex}");
        }
    }
    public static async Task SeedPizzaAsync(PizzaDbContext dbContext)
    {
        var path = Path.Combine("..","Pizza","Data","Pizzas.json");
        string pizzaJson = File.ReadAllText(path);
        try
        {
            var pizzas = JsonSerializer.Deserialize<List<Models.Pizza>>(pizzaJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() } 
                });
            if (pizzas != null && pizzas.Count > 0)
            {
                foreach (var pizza in pizzas)
                {
                    if (await dbContext.Pizzas.AnyAsync(e => pizza.Name == e.Name) == false)
                    {
                        await dbContext.Pizzas.AddAsync(pizza);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization failed: {ex}");
        }
    }
    public static async Task SeedCouponsAsync(PizzaDbContext dbContext)
    {
        var path = Path.Combine("..","Pizza","Data","Coupons.json");
        string couponJson = File.ReadAllText(path);
        try
        {
            var coupons = JsonSerializer.Deserialize<List<Coupon>>(couponJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() } 
                });
            if (coupons != null && coupons.Count > 0)
            {
                foreach (var coupon in coupons)
                {
                    if (await dbContext.Coupons.AnyAsync(e => coupon.Name == e.Name) == false)
                    {
                        await dbContext.Coupons.AddAsync(coupon);
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization failed: {ex}");
        }
    }
}