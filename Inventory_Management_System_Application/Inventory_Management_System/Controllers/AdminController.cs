using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Inventory_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Inventory_Management_System.Interfaces;

using Inventory_Management_System.Service;

namespace Inventory_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPhotoService _photoService;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext, IPhotoService photoService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _photoService = photoService;
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
        public async Task<IActionResult> AddProduct(ProductView productView)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = productView.ProductName,
                    ProductDescription = productView.ProductDescription,
                    ProductPrice = productView.ProductPrice,
                    ProductQuantity = productView.ProductQuantity,
                    //Supplier = productView.Supplier,
                    CreatedBy = productView.CreatedBy,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                if (productView.ProductImg != null && productView.ProductImg.Length > 0)
                {
                    var uploadResult = await _photoService.AddPhotoAsync(productView.ProductImg);
                    if (uploadResult.Error == null)
                    {
                        product.ProductImg = uploadResult.Url.AbsoluteUri; // Save the image URL in the database
                    }
                    else
                    {
                        ModelState.AddModelError("ProductImgFile", "Failed to upload image.");
                        return View(productView);
                    }
                }
                product.CreatedDate = DateTime.Now;
                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Products));
            }

            return View(productView);
        }

        // GET: Admin/EditProduct/{id}
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Product_Model.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/EditProduct/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing product from the database
                    var existingProduct = await _dbContext.Product_Model.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing product except CreatedAt
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.ProductDescription = product.ProductDescription;
                    existingProduct.ProductPrice = product.ProductPrice;
                    existingProduct.ProductQuantity = product.ProductQuantity;
                    //existingProduct.Supplier = product.Supplier;
                    existingProduct.CreatedBy = product.CreatedBy;
                    existingProduct.UpdatedDate = DateTime.Now; // Update the UpdatedDate

                    _dbContext.Update(existingProduct);
                    await _dbContext.SaveChangesAsync();
                
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Products));
            }

            return View(product);
        }

        // GET: Admin/ConfirmDeleteProduct/{id}
        public async Task<IActionResult> ConfirmDeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Product_Model.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/DeleteProduct/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _dbContext.Product_Model.FindAsync(id);
            _dbContext.Product_Model.Remove(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }

        private bool ProductExists(int id)
        {
            return _dbContext.Product_Model.Any(e => e.ProductID == id);
        }

        // GET: Admin/Suppliers
        public async Task<IActionResult> Suppliers()
        {
            // Retrieve all suppliers from the database
            var suppliers = await _dbContext.Supplier_Model.ToListAsync();
            return View(suppliers);
        }

        // GET: Admin/AddSupplier
        public IActionResult AddSupplier()
        {
            return View();
        }

        // POST: Admin/AddSupplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            // Create the supplier object
            supplier.CreatedAt = DateTime.Now;
           

            // Save the supplier to the database
            if (ModelState.IsValid)
            {
                _dbContext.Add(supplier);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Suppliers));
            }

            return View(supplier);
        }

        // GET: Admin/EditSupplier/{id}
        [HttpGet]
        public async Task<IActionResult> EditSupplier(int? id)
        {
            // Retrieve the supplier from the database using the id
            var supplier = await _dbContext.Supplier_Model.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            supplier.CreatedAt = supplier.CreatedAt;


            return View(supplier);
        }

        // POST: Admin/EditSupplier/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, Supplier supplier)
        {
            if (id != supplier.SupplierID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing product from the database
                    var existingSupplier = await _dbContext.Supplier_Model.FindAsync(id);
                    if (existingSupplier == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing product except CreatedAt
                    existingSupplier.SupplierName = supplier.SupplierName;
                    existingSupplier.Location = supplier.Location;
                    existingSupplier.Products = supplier.Products;
                    existingSupplier.ContactDetails = supplier.ContactDetails;
                    existingSupplier.CreatedBy = supplier.CreatedBy;
                    existingSupplier.UpdatedAt = DateTime.Now; // Update the UpdatedDate

                    _dbContext.Update(existingSupplier);
                    await _dbContext.SaveChangesAsync();
       
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Suppliers));
            }

            return View(supplier);
        }

        // GET: Admin/ConfirmDeleteSupplier/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDeleteSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _dbContext.Supplier_Model.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Admin/DeleteSupplier/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _dbContext.Supplier_Model.FindAsync(id);
            _dbContext.Supplier_Model.Remove(supplier);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Suppliers));
        }

        private bool SupplierExists(int id)
        {
            return _dbContext.Supplier_Model.Any(e => e.SupplierID == id);
        }

    }
}