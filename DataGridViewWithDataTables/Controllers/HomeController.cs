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
                                query = query.Where(p => p.Name.ToLower().Contains(searchValue));
                                break;
                            case "price":
                                query = query.Where(p => p.Price.ToString().Contains(searchValue));
                                break;
                        }
                    }
                }

                var totalRecords = _context.Products.Count();
                var filteredCount = query.Count();

                //// Sıralama
                //if (!string.IsNullOrEmpty(request.SortColumn))
                //{
                //    var isAscending = request.SortDirection?.ToLower() == "asc";
                //    switch (request.SortColumn.ToLower())
                //    {
                //        case "id":
                //            query = isAscending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id);
                //            break;
                //        case "name":
                //            query = isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                //            break;
                //        case "price":
                //            query = isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                //            break;
                //    }
                //}

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
