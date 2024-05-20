using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Constants;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace Inventory_Management_System.Controllers.API
{
    [Authorize(Roles = "Client")]
    [Route("api/[controller]")]  
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public ClientController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;

        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderViewModel orderRequest)
        {
            // Validate the order request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Get the cart items for the current user
            var cartItems = await _dbContext.CartItem_Model
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Your cart is empty.");

            // Create a new order
            var order = new Order
            {
                UserId = user.Id,
                OrderCreatedAt = DateTime.UtcNow,
                OrderDate = orderRequest.OrderDate,
                OrderPlacedBy = user.UserName,
                ShippingAddress = orderRequest.ShippingAddress,
                OrderStatus = "Pending",
                TotalAmount = cartItems.Sum(c => c.Product.ProductPrice * c.Quantity)
            };

            // Add the order to the database
            _dbContext.Order_Model.Add(order);

            // Save changes to get the OrderId
            await _dbContext.SaveChangesAsync();

            // Create order items based on the cart items and update product quantities
            foreach (var cartItem in cartItems)
            {
                if (cartItem.Quantity > cartItem.Product.ProductQuantity)
                {
                    return BadRequest($"Insufficient quantity for product {cartItem.Product.ProductName}.");
                }

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.ProductPrice
                };

                cartItem.Product.ProductQuantity -= cartItem.Quantity;

                _dbContext.OrderItem_Model.Add(orderItem);
                _dbContext.Product_Model.Update(cartItem.Product); // Update the product quantity
            }

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            // Delete the cart items for the current user
            await DeleteCartItems(user);

            return Ok(order);
        }

        private async Task DeleteCartItems(ApplicationUser user)
        {
            // Get the cart items for the current user
            var cartItems = _dbContext.CartItem_Model.Where(c => c.UserId == user.Id);

            // Remove the cart items from the database
            _dbContext.CartItem_Model.RemoveRange(cartItems);
            await _dbContext.SaveChangesAsync();
        }






        [HttpPost("CancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Retrieve the order by Id and ensure it belongs to the current user and is pending
            var order = await _dbContext.Order_Model
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == user.Id && o.OrderStatus == "Pending");

            if (order == null)
            {
                return NotFound("Order not found or already processed.");
            }

            // Retrieve the order items associated with the order
            var orderItems = await _dbContext.OrderItem_Model
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ToListAsync();

            // Update order status to "Canceled"
            order.OrderStatus = "Canceled";
            order.OrderCreatedAt = DateTime.UtcNow; // Use UTC time for consistency
            _dbContext.Order_Model.Update(order);

            // Restore product quantities
            foreach (var orderItem in orderItems)
            {
                var product = orderItem.Product;
                product.ProductQuantity += orderItem.Quantity;
                _dbContext.Product_Model.Update(product);
            }

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            return Ok("Order has been canceled.");
        }
    }
}
