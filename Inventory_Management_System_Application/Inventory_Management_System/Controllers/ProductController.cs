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
