using System;
using System.Linq;
using System.Threading.Tasks;
using Inventory_Management_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Service
{
    public class CartService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public int CartItemCount { get; private set; }

        public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            CartItemCount = 0;


        }

        public async Task<int> GetCartItemCountAsync()
        {
            try
            {
                var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
                }

                var cartItemCount = await _dbContext.CartItem_Model
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);

                return cartItemCount;
            }
            catch (Exception ex)
            {
                // Log or handle the exception here
                throw new Exception("Error retrieving cart item count", ex);
            }
        }

        // Add method to update cart count asynchronously
        public async Task UpdateCartItemCountAsync()
        {
            // Call GetCartItemCountAsync to update the CartItemCount property
            CartItemCount = await GetCartItemCountAsync();
        }

    }
}
