using BaseMockDatabaseSqlLite.Data;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace BaseMockDatabaseSqlLite.SeedData
{
    public static class SeedDataClass
    {
        // how to use get required service 

     

        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await SeedProductsAsync(context);
            }
        }
        

        // seed products
        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                var products = new Faker<Product>()
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
                    .RuleFor(p=>p.Description , f=>f.Commerce.ProductDescription())
                    .Generate(100);

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
           
        }

    }

}