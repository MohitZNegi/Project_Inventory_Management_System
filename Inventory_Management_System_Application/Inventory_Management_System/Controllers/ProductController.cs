using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Models;
using System.Threading.Tasks;
using Inventory_Management_System.Areas.Identity.Data;

namespace Inventory_Management_System.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Products(string searchQuery, int page = 1)
        {
            // Retrieve all products from your database or data source
            List<Product> allProducts = _dbContext.Product_Model.ToList();

            // Filter products based on the search query
            var filteredProducts = string.IsNullOrEmpty(searchQuery)
                ? allProducts
                : allProducts.Where(p => p.ProductName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

            // Apply pagination to the filtered products
            var pageSize = 16;
            var pageCount = (int)Math.Ceiling(filteredProducts.Count() / (double)pageSize);
            var productsOnPage = filteredProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass the filtered and paginated products to the view
            ViewData["SearchQuery"] = searchQuery;
            ViewData["PageCount"] = pageCount;
            ViewData["CurrentPage"] = page;
            return View(productsOnPage);
        }


        public async Task<IActionResult> ProductItems(int? id)
        {
            var product = await _dbContext.Product_Model.FindAsync(id);

            if (product == null)
            {
                return NotFound(); // Handle case where product is not found
            }

            // Pass the product to the view
            return View(product);
        }
    }
}
