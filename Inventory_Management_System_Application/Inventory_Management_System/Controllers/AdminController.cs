using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Inventory_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
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
           // var products = _dbContext.Product_Model.Include(p => p.ProductSuppliers).ToList();
            var products = await _dbContext.Product_Model.ToListAsync();
            return View(products);
        }

        // GET: Admin/AddProduct
        public IActionResult AddProduct()
        {
            var suppliers = _dbContext.Supplier_Model.ToList();

            var supplierList = suppliers.Select(s => new SelectListItem
            {
                Text = s.SupplierName,
                Value = $"{s.SupplierID}-{s.SupplierName}" // Combine SupplierID and SupplierName
            });

            ViewBag.Suppliers = supplierList;
          /*  // Retrieve a list of suppliers from the database
            var suppliers = _dbContext.Supplier_Model.ToList();
            // Create a SelectList with SupplierName as the text and SupplierID as the value
            var supplierList = suppliers.Select(s => new SelectListItem
            {
                Text = s.SupplierName,
                Value = s.SupplierID.ToString()
            }) ;

            // Pass the SelectList to the view
            ViewBag.Suppliers = supplierList;
            
            // Create a SelectList with SupplierID as the text and SupplierID as the value
            var supplierIDList = suppliers.Select(s => new SelectListItem
            {
                Text = s.SupplierID.ToString(),
                Value = s.SupplierID.ToString()
            });

            // Pass the SelectList to the view
            ViewBag.SuppliersID = supplierIDList;*/

            return View();
        }

        // POST: Admin/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product, string supplier)
        {
            product.CreatedDate = DateTime.Now;
            // product.SupplierID = SupplierID;
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

            if (!ModelState.IsValid)
            {
                // Retrieve SupplierID from the selected supplier string
                var supplierID = int.Parse(supplier.Split('-')[0]);
                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();

                // Update the supplier's product list
                await UpdateSupplierProductList(supplierID, product.ProductName);
                return RedirectToAction(nameof(Products));

            }

            return View(product);
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
        public async Task<IActionResult> EditProduct(int id, Product product, string supplier)
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

                    // Update the properties of the existing product except CreatedAt
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.ProductDescription = product.ProductDescription;
                    existingProduct.ProductPrice = product.ProductPrice;
                    existingProduct.ProductQuantity = product.ProductQuantity;
                    existingProduct.ProductSuppliers = product.ProductSuppliers;
                    existingProduct.SupplierID = product.SupplierID;
                    existingProduct.CreatedBy = product.CreatedBy;
                    existingProduct.UpdatedDate = DateTime.Now; // Update the UpdatedDate

                  

                    _dbContext.Update(existingProduct);
                    await _dbContext.SaveChangesAsync();

                    // Update the supplier's product list
                    await UpdateSupplierProductList(supplierID, product.ProductName);

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
        private async Task UpdateSupplierProductList(int supplierID, string productName)
        {
            var supplier = await _dbContext.Supplier_Model.FindAsync(supplierID);
            if (supplier != null)
            {
                // Append the product name to the existing Products string, separated by comma or any other delimiter
                supplier.Products += string.IsNullOrEmpty(supplier.Products) ? productName : "," + productName;
                _dbContext.Update(supplier);
            
                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }
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