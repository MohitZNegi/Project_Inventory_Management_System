using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Inventory_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Inventory_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Retrieve client details from the database
            var user = _dbContext.Users.ToList();
            var clients = _dbContext.Client_Model.ToList();
            return View(clients);
        }

        // GET: Admin/Products
        public async Task<IActionResult> Products()
        {
            // Retrieve all products from the database
            var products = await _dbContext.Product_Model.ToListAsync();
            return View(products);
        }
        // GET: Admin/AddProduct
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: Admin/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductView PV)
        {
            
            

            // Create the product object
            var product = new Product
            {
                ProductName = PV.ProductName,
                ProductDescription = PV.ProductDescription,
                ProductPrice = PV.ProductPrice,
                ProductQuantity = PV.ProductQuantity,
                CreatedBy = PV.CreatedBy,
                Supplier = PV.Supplier,
                //  ProductImg = PV.ProductImg,
                CreatedDate = DateTime.Now,
              
            };

            // Save the product to the database
            if (ModelState.IsValid)
            {
                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Products", "Admin");
            }

            return View(product);
        }

       // GET: Admin/EditProduct/{id}
        [HttpGet]
        public async Task<IActionResult> EditProduct(int? Id)
        {
            // Retrieve the product from the database using the id

            var product = _dbContext.Product_Model.Find(Id);

            if (product == null)
            {
                return NotFound();
            }

            // Map the product to a ProductViewModel
            var productViewModel = new Product
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductQuantity = product.ProductQuantity,
                Supplier = product.Supplier,
                CreatedBy = product.CreatedBy,
                UpdatedDate = product.UpdatedDate
            // No need to map ProductImg in Edit action
        };

            return View(productViewModel);
        }

        // POST: Admin/EditProduct/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int? id, ProductView productViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }

            var product = _dbContext.Product_Model.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                

                    // Update the properties of the existing product
                    product.ProductName = productViewModel.ProductName;
                    product.ProductDescription = productViewModel.ProductDescription;
                    product.ProductPrice = productViewModel.ProductPrice;
                    product.ProductQuantity = productViewModel.ProductQuantity;
                    product.Supplier = productViewModel.Supplier;
                    product.CreatedBy = productViewModel.CreatedBy;
                    product.UpdatedDate = DateTime.Now;

                    // Save the changes to the database
                   _dbContext.SaveChanges();

                    return RedirectToAction("Products", "Admin");
                }
                catch (Exception)
                {
                    // Handle exceptions
                    throw;
                }
            }

            return View(productViewModel);
        }

        // GET: Admin/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _dbContext.Product_Model.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        // POST: Admin/DeleteProduct/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            var product = _dbContext.Product_Model.Find(id);

            if (product == null)
            {
                return NotFound();
            }
            _dbContext.Product_Model.Remove(product);
            _dbContext.SaveChanges();
           return RedirectToAction("Products", "Admin");
        }



    }
}