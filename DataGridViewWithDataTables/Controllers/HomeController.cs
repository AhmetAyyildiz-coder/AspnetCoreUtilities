using BaseLibrary.Models.FrontEnd;
using BaseMockDatabaseSqlLite.Data;
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

        //protected readonly List<Product> _mockProducts;
        private ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, IMemoryCache cache, ApplicationDbContext context)
        {
            _logger = logger;
            _cache = cache;
            _context = context;
        }


        [HttpPost]
        public IActionResult GetProducts([FromBody] DataTableRequest request)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                // Global arama
                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    var searchValue = request.SearchValue.ToLower();
                    query = query.Where(p =>
                        p.Id.ToString().Contains(searchValue) ||
                        p.Name.ToLower().Contains(searchValue) ||
                        p.Price.ToString().Contains(searchValue));
                }

                // Kolon bazlı filtreleme
                if (request.Columns != null)
                {
                    foreach (var column in request.Columns.Where(c => !string.IsNullOrEmpty(c.SearchValue)))
                    {
                        var searchValue = column.SearchValue.ToLower();
                        switch (column.Data.ToLower())
                        {
                            case "id":
                                query = query.Where(p => p.Id.ToString().Contains(searchValue));
                                break;
                            case "name":
                                var splittedNames = searchValue.Split("|");
                                query = query.Where(p =>
                                    splittedNames.Any(name => p.Name.ToLower() == name.ToLower()
                                    )
                                );
                                break;
                            case "price":
                                query = query.Where(p => p.Price.ToString().Contains(searchValue));
                                break;
                        }
                    }
                }

                var totalRecords = _context.Products.Count();
                var filteredCount = query.Count();

               
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
