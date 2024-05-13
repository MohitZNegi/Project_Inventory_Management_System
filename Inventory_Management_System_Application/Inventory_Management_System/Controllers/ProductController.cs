using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Models;
using System.Threading.Tasks;
using Inventory_Management_System.Areas.Identity.Data;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
               
        }
        public async Task<IActionResult> ProductItems(int id)
        {
            // Retrieve the product from the database using the productId
            var product = await _dbContext.Product_Model.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            // Get the current user ID
            var userId = _userManager.GetUserId(HttpContext.User);

            // Retrieve the product from the database using the productId
            var product =  await _dbContext.Product_Model.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Retrieve the cart items for the current user
            var existingCartItem = await _dbContext.CartItem_Model
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == id);

            if (existingCartItem != null)
            {
                // If the product already exists in the cart, update the quantity
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // If the product is not already in the cart, add it with the specified quantity
                var newCartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = id,
                    Quantity = quantity
                };

                // Add the new cart item to the database
                _dbContext.CartItem_Model.Add(newCartItem);
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Return a JSON response indicating success
            return Json(new { success = true });

        }
        public IActionResult ViewCart()
        {
            // Get the current user ID
            var userId = _userManager.GetUserId(HttpContext.User);

            // Retrieve cart items for the current user, including the related product
            var cartItems = _dbContext.CartItem_Model
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();
            return View(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> EditCartItem(int id, int quantity)
        {
            // Retrieve the cart item from the database
            var cartItem = await _dbContext.CartItem_Model.FindAsync(id);
            if (cartItem != null)
            {
                // Update the quantity of the cart item
                cartItem.Quantity = quantity;

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                // Return a JSON response indicating success
                return Json(new { success = true });
            }

            // If the cart item is not found, return a JSON response indicating failure
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            // Retrieve the cart item from the database
            var cartItem = await _dbContext.CartItem_Model.FindAsync(id);
            if (cartItem != null)
            {
                // Remove the cart item from the database
                _dbContext.CartItem_Model.Remove(cartItem);

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                // Return a JSON response indicating success
                return Json(new { success = true });
            }

            // If the cart item is not found, return a JSON response indicating failure
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            // Retrieve all cart items for the current user from the database
            var userId = _userManager.GetUserId(HttpContext.User);
            var cartItems = await _dbContext.CartItem_Model
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Remove all cart items from the database
            _dbContext.CartItem_Model.RemoveRange(cartItems);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            // Return a JSON response indicating success
            return Json(new { success = true });
        }
    }
}
