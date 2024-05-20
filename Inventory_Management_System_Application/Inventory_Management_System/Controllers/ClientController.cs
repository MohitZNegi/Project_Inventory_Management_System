using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Inventory_Management_System.ViewModels;
using SendGrid.Helpers.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Core;

namespace Inventory_Management_System.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
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

        public IActionResult ViewProfile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            var profile = _dbContext.Client_Model.Where(e => e.Client_Id == userId).FirstOrDefault();


            if (profile == null)
            {
                return RedirectToAction("Create", "Client");
            }

            var viewModel = new Client
            {

                First_name = profile.First_name,
                Last_name = profile.Last_name,


            };

            return View(viewModel);




        }
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var existingProfile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (existingProfile != null)
            {
                return Content("You have already created a profile!");
            }

            var viewModel = new ClientView();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientView clientVM)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var existingProfile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (ModelState.IsValid && existingProfile == null)
            {
                var profile = new Client
                {
                    UserId = userId,
                    Client_Id = userId,
                    First_name = clientVM.FirstName,
                    Last_name = clientVM.LastName,
                    // Add other properties
                };

                await _dbContext.Client_Model.AddAsync(profile);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(clientVM);
        }

        public IActionResult Update()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var profile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (profile == null)
            {
                return RedirectToAction("Create", "Client");
            }

            var viewModel = new ClientView
            {
                FirstName = profile.First_name,
                LastName = profile.Last_name,

                // Add other properties
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(ClientView clientVM)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var profile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (ModelState.IsValid && profile != null)
            {
                profile.First_name = clientVM.FirstName;
                profile.Last_name = clientVM.LastName;
                // Update other properties

                _dbContext.Update(profile);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(clientVM);
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
            var product = await _dbContext.Product_Model.FindAsync(id);
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

        public IActionResult Checkout()
        {
            // Get the current user
            var user = _userManager.GetUserAsync(User).Result;
            
            var userId = _userManager.GetUserId(HttpContext.User);
            var userName = _userManager.GetUserName(HttpContext.User);
            ViewBag.CurrentUser = userId;
            // Get the cart items for the current user
            var cartItems = _dbContext.CartItem_Model
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToList();

            // Calculate the total amount
            var totalAmount = cartItems.Sum(c => c.Product.ProductPrice * c.Quantity);

            // Pass the cart items and total amount to the view
            ViewBag.CartItems = cartItems;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.CurrentUserName = userName;

            // Create a new instance of the OrderRequest model
            var orderRequest = new Order();

            return View(orderRequest);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Retrieve the order history for the user, including related order items and product details
            var orders = await _dbContext.OrderItem_Model
                .Where(o => o.Order.UserId == user.Id)
                .Include(o => o.Order)
                .Include(o => o.Product)
                .ToListAsync();

            // Group the order items by the order they belong to
            var groupedOrders = orders.GroupBy(o => o.OrderId)
                                      .Select(g => g.First().Order) // Select only the unique orders
                                      .ToList();

            return View(groupedOrders);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Retrieve the order from the database
            var order = await _dbContext.Order_Model
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Retrieve order items associated with the order
            var orderItems = await _dbContext.OrderItem_Model
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            // Map the order and order items to a view model
            var orderDetails = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                UserName = order.User.UserName,
                OrderCreatedAt = order.OrderCreatedAt,
                OrderDate = order.OrderDate,
                OrderPlacedBy = order.OrderPlacedBy,
                ShippingAddress = order.ShippingAddress,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                OrderItems = orderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return View(orderDetails);
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceList()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();
            var invoices = await _dbContext.Invoice_Model
                .Where(o => o.Order.UserId == user.Id)
                .Include(i => i.Order)
                .ThenInclude(o => o.User)
                .Select(i => new InvoiceViewModel
                {
                    InvoiceId = i.InvoiceId,
                    OrderId = i.OrderId,
                    UserName = i.Order.User.UserName,
                    OrderDate = i.Order.OrderDate,
                    DueDate = i.DueDate,
                    InvoiceFilePath = i.InvoiceFilePath,
                    PaymentStatus = i.PaymentStatus,
                    TotalAmount = i.Order.TotalAmount
                })
                .ToListAsync();

            return View(invoices);
        }
    }
}