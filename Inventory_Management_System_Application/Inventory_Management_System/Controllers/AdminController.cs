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
using Microsoft.AspNetCore.Mvc.Rendering;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.CodeAnalysis;
using SendGrid.Helpers.Mail;
using Inventory_Management_System.Migrations;

namespace Inventory_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPhotoService _photoService;
        private readonly Cloudinary _cloudinary;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext, IPhotoService photoService, Cloudinary cloudinary)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _photoService = photoService;
            _cloudinary = cloudinary;
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
            // var products = _dbContext.Product_Model.Include(p => p.ProductSuppliers).ToList();
            var products = await _dbContext.Product_Model.ToListAsync();
            return View(products);
        }

        // GET: Admin/AddProduct
        public IActionResult AddProduct()
        {
            var suppliers = _dbContext.Supplier_Model.ToList();

            if (suppliers != null)
            {
                var supplierList = suppliers.Select(s => new SelectListItem
                {
                    Text = s.SupplierName,
                    Value = $"{s.SupplierID}-{s.SupplierName}" // Combine SupplierID and SupplierName
                }).ToList(); // Explicitly convert to List<SelectListItem>

                var productView = new ProductView
                {
                    Suppliers = supplierList
                };

                return View(productView);
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddProduct(ProductView productView, string supplier)
        {
            if (ModelState.IsValid)
            {
                // Retrieve suppliers from the database
                var suppliers = await _dbContext.Supplier_Model.ToListAsync();

                if (suppliers != null && suppliers.Count > 0)
                {
                    // Create a SelectList for suppliers
                    var supplierSelectList = new SelectList(suppliers, "SupplierID", "SupplierName");

                    var product = new Product
                    {
                        // Map properties from ProductView to Product
                        ProductName = productView.ProductName,
                        ProductDescription = productView.ProductDescription,
                        ProductPrice = productView.ProductPrice,
                        ProductQuantity = productView.ProductQuantity,
                        CreatedBy = productView.CreatedBy,
                        ProductSuppliers = productView.ProductSuppliers,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                    };
                    // Retrieve SupplierID from the selected supplier string
                    var supplierId = int.Parse(supplier.Split('-')[0]);


                    // Check if a supplier is selected
                    if (!string.IsNullOrEmpty(productView.Supplier))
                    {
                        // Try parsing the Supplier ID from the productView.Supplier string
                        if (int.TryParse(productView.Supplier.Split('-')[0], out int supplierID))
                        {
                            // Find the selected supplier
                            var selectedSupplier = await _dbContext.Supplier_Model.FindAsync(supplierID);
                            if (selectedSupplier != null)
                            {
                                // Assign the selected supplier to the product.Supplier property
                                product.Supplier = selectedSupplier;
                                product.ProductSuppliers = selectedSupplier.SupplierName; // Set ProductSuppliers property
                            }
                        }
                    }



                    // Update the supplier's product list
                    await UpdateSupplierProductList(supplierId, null, product.ProductName);

                    // Handle product image upload if available
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

                    // Save the product to the database
                    _dbContext.Add(product);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Products));
                }


            }

            // If ModelState is not valid or suppliers are empty, return the view with the model
            return View(productView);
        }


        // GET: Admin/EditProduct/{id}
        public async Task<IActionResult> EditProduct(int? id)
        {
            var suppliers = _dbContext.Supplier_Model.ToList();
            var product = await _dbContext.Product_Model.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            int selectedSupplierID = product.SupplierID;
            var supplierList = suppliers.Select(s => new SelectListItem
            {
                Text = s.SupplierName,
                Value = $"{s.SupplierID}-{s.SupplierName}", // Combine SupplierID and SupplierName
                Selected = s.SupplierID == product.SupplierID // Set selected supplier
            });

            ViewBag.Suppliers = supplierList;



            if (id == null)
            {
                return NotFound();
            }


            return View(product);
        }

        // POST: Admin/EditProduct/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product, string supplier, IFormFile productImgFile)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    // Retrieve SupplierID from the selected supplier string
                    var supplierID = int.Parse(supplier.Split('-')[0]);

                    // Check if the supplier value is not null or empty
                    if (!string.IsNullOrEmpty(supplier))
                    {
                        // Split the selected value to extract SupplierID and SupplierName
                        var supplierParts = supplier.Split('-');
                        if (supplierParts.Length == 2)
                        {
                            product.SupplierID = int.Parse(supplierParts[0]);
                            product.ProductSuppliers = supplierParts[1];
                        }
                    }

                    // Retrieve the existing product from the database
                    var existingProduct = await _dbContext.Product_Model.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }
                    // Get the old supplier ID before updating the product
                    var oldSupplierID = existingProduct.SupplierID;
                    // Update the properties of the existing product except CreatedAt
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.ProductDescription = product.ProductDescription;
                    existingProduct.ProductPrice = product.ProductPrice;
                    existingProduct.ProductQuantity = product.ProductQuantity;
                    existingProduct.ProductSuppliers = product.ProductSuppliers;
                    existingProduct.SupplierID = product.SupplierID;
                    existingProduct.CreatedBy = product.CreatedBy;
                    existingProduct.UpdatedDate = DateTime.Now; // Update the UpdatedDate

                    // Check if the supplier value is not null or empty
                    if (!string.IsNullOrEmpty(supplier))
                    {
                        // Split the selected value to extract SupplierID and SupplierName
                        var supplierParts = supplier.Split('-');
                        if (supplierParts.Length == 2)
                        {
                            product.SupplierID = int.Parse(supplierParts[0]);
                            product.ProductSuppliers = supplierParts[1];
                        }
                    }

                    if (productImgFile != null && productImgFile.Length > 0)
                    {
                        // Save the file to a storage location (e.g., using a service like _photoService)
                        var uploadResult = await _photoService.AddPhotoAsync(productImgFile);
                        if (uploadResult.Error == null)
                        {
                            // Update the ProductImg property with the URL of the uploaded image
                            existingProduct.ProductImg = uploadResult.Url.AbsoluteUri; // or the path if stored locally
                        }
                        else
                        {
                            ModelState.AddModelError("ProductImgFile", "Failed to upload image.");
                            return View(product);
                        }
                    }
                    else
                    {
                        // If no new image is uploaded, retain the existing image
                        product.ProductImg = existingProduct.ProductImg;
                    }

                    _dbContext.Update(existingProduct);
                    await _dbContext.SaveChangesAsync();



                    // Update the supplier's product list
                    await UpdateSupplierProductList(product.SupplierID, oldSupplierID, product.ProductName);


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

        // Helper method to update the Products property of the supplier
        private async Task UpdateSupplierProductList(int newSupplierID, int? oldSupplierID, string productName)
        {
            // If the product was previously associated with a supplier, remove it from that supplier's product list
            if (oldSupplierID.HasValue)
            {
                var oldSupplier = await _dbContext.Supplier_Model.FindAsync(oldSupplierID);
                if (oldSupplier != null)
                {
                    // Remove the product name from the existing Products string
                    oldSupplier.Products = string.Join(",", oldSupplier.Products.Split(',').Where(p => p != productName));
                    _dbContext.Update(oldSupplier);
                }
            }

            // If newSupplierID is not null, update the product list of the new supplier
            if (newSupplierID != 0)
            {
                var supplier = await _dbContext.Supplier_Model.FindAsync(newSupplierID);
                if (supplier != null)
                {
                    // Append the product name to the existing Products string, separated by comma or any other delimiter
                    supplier.Products += string.IsNullOrEmpty(supplier.Products) ? productName : "," + productName;
                    _dbContext.Update(supplier);
                }
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
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
            // Remove the product from the supplier's product list
            await RemoveProductFromSupplier(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }

        // Helper method to remove the product from the supplier's product list
        private async Task RemoveProductFromSupplier(Product product)
        {
            var suppliers = await _dbContext.Supplier_Model.Where(s => s.Products.Contains(product.ProductName)).ToListAsync();
            foreach (var supplier in suppliers)
            {
                // Remove the product name from the supplier's product list
                supplier.Products = string.Join(",", supplier.Products.Split(',').Where(p => p != product.ProductName));
                _dbContext.Update(supplier);
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }

        private bool ProductExists(int id)
        {
            return _dbContext.Product_Model.Any(e => e.ProductID == id);
        }

        // GET: Admin/Suppliers
        public async Task<IActionResult> Suppliers()
        {
            // Retrieve all suppliers from the database
            //var suppliers = _dbContext.Supplier_Model.Include(s => s.ProductSuppliers).ToList();
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
            // Retrieve all products associated with the supplier
            var productsToDelete = await _dbContext.Product_Model.Where(p => p.SupplierID == id).ToListAsync();

            // Delete all associated products
            _dbContext.Product_Model.RemoveRange(productsToDelete);
            _dbContext.Supplier_Model.Remove(supplier);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Suppliers));
        }

        private bool SupplierExists(int id)
        {
            return _dbContext.Supplier_Model.Any(e => e.SupplierID == id);
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

            int cartItemCount = cartItems.Sum(c => c.Quantity);

            // Pass the cart item count to the view
            ViewBag.CartItemCount = cartItemCount;

            return View(cartItems);
        }

    }
        
}