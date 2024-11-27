using BaseLibrary.Models.FrontEnd;
using Bogus;
using DataGridViewWithDataTables.Models;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Linq;

namespace DataGridViewWithDataTables.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;


        private const string CACHE_KEY = "GridMockData";
        private const int CACHE_DURATION_MINUTES = 5;

        private static List<Product> _mockProducts;


        public HomeController(ILogger<HomeController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;

            var faker = new Faker<Product>()
               .RuleFor(p => p.Id, f => f.IndexFaker + 1)
               .RuleFor(p => p.Name, f => f.Commerce.ProductName())
               .RuleFor(p => p.Category, f => f.Commerce.ProductMaterial())
               .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)))
               .RuleFor(p => p.Description, f => f.Commerce.ProductDescription());

            _mockProducts = faker.Generate(500);
        }


        [HttpPost]
        public IActionResult GetProducts([FromBody] DataTableRequest request)
        {
            try
            {
                var query = _mockProducts.AsQueryable();

                // Global arama - case insensitive
                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    var searchValue = request.SearchValue.ToLower();
                    query = query.Where(p =>
                        p.Id.ToString().Contains(searchValue) ||
                        p.Name.ToLower().Contains(searchValue) ||
                        p.Price.ToString().Contains(searchValue) ||
                        (p.Category != null && p.Category.ToLower().Contains(searchValue))
                    );
                }

                var totalRecords = _mockProducts.Count;
                var filteredCount = query.Count();

                // Sýralama
                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    var isAscending = request.SortDirection?.ToLower() == "asc";
                    query = request.SortColumn.ToLower() switch
                    {
                        "id" => isAscending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id),
                        "name" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                        "price" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                        "category" => isAscending ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
                        _ => query
                    };
                }

                // Sayfalama
                var data = query.Skip(request.Start)
                               .Take(request.Length)
                               .ToList();

                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredCount,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
