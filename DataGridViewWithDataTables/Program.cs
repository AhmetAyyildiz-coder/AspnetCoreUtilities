using BaseMockDatabaseSqlLite.Data;
using BaseMockDatabaseSqlLite.SeedData;
using DataTables.AspNet.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache(options =>
{
    // Cache boyutunu sınırla (opsiyonel)
    options.SizeLimit = 1024; //mb

    // Cache dolu olduğunda en az kullanılan sil
    options.CompactionPercentage = 0.25;
});

try
{
    SeedDataClass.SeedDatabaseAsync(builder.Services.BuildServiceProvider()).Wait();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
