using Inventory_Management_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.ViewModels;
using CloudinaryDotNet.Actions;
using Inventory_Management_System.Models;
using System.Drawing;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MimeKit;
using Font = iTextSharp.text.Font;
using Paragraph = iTextSharp.text.Paragraph;
using Microsoft.AspNetCore.Hosting;

namespace Inventory_Management_System.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost("UpdateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            var order = await _dbContext.Order_Model.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (order.OrderStatus == "Canceled")
            {
                return BadRequest("Cannot update the status of a canceled order.");
            }

            order.OrderStatus = newStatus;
            _dbContext.Order_Model.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok("Order status updated successfully.");
        }
        [HttpPost("ConfirmDelivery/{orderId}")]
        public async Task<IActionResult> ConfirmDelivery(int orderId)
        {
            var order = await _dbContext.Order_Model
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            if (order.OrderStatus == "Delivered" || order.OrderStatus == "Canceled")
            {
                return BadRequest("Cannot update the status of a delivered or canceled order.");
            }

            order.OrderStatus = "Delivered";
            _dbContext.Order_Model.Update(order);
            await _dbContext.SaveChangesAsync();

            var orderItems = await _dbContext.OrderItem_Model
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            var invoiceFilePath = GenerateInvoice(order, orderItems);

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
               
                OrderDate = order.OrderDate,
                DueDate = order.OrderDate.AddDays(30),
                InvoiceFilePath = invoiceFilePath,
                PaymentStatus = "Pending"
            };

            _dbContext.Invoice_Model.Add(invoice);
            await _dbContext.SaveChangesAsync();

            return Ok("Order delivery confirmed. Invoice generated and saved.");
        }

        private string GenerateInvoice(Order order, List<OrderItem> orderItems)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var invoicesPath = Path.Combine(wwwRootPath, "invoices");
            if (!Directory.Exists(invoicesPath))
            {
                Directory.CreateDirectory(invoicesPath);
            }

            var invoiceFileName = $"Invoice_{order.OrderId}.pdf";
            var invoiceFilePath = Path.Combine(invoicesPath, invoiceFileName);

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, new FileStream(invoiceFilePath, FileMode.Create));
                document.Open();

                var logoUrl = "https://res.cloudinary.com/dup5hdi05/image/upload/v1715060622/ware-master-high-resolution-logo-transparent_1_vj1owp.png";
                var logo = iTextSharp.text.Image.GetInstance(logoUrl);
                logo.ScaleAbsolute(200f, 100f); 
                document.Add(logo);

                document.Add(new Paragraph($"Invoice for Order #{order.OrderId}", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                document.Add(new Paragraph($"Order Date: {order.OrderDate:MM/dd/yyyy}"));
                document.Add(new Paragraph($"Payment Due Date: {order.OrderDate.AddDays(30):MM/dd/yyyy}")); // Assuming a 30-day payment period
                document.Add(new Paragraph($"Client: {order.User.UserName}"));
                document.Add(new Paragraph($"Client Id: {order.User.Id}"));
                document.Add(new Paragraph($"Shipping Address: {order.ShippingAddress}"));
                document.Add(new Paragraph($"Order Status: {order.OrderStatus}"));
                document.Add(new Paragraph(" "));

                var table = new PdfPTable(3);
                table.AddCell("Product Name");
                table.AddCell("Quantity");
                table.AddCell("Price");

                foreach (var item in orderItems)
                {
                    table.AddCell(item.Product.ProductName);
                    table.AddCell(item.Quantity.ToString());
                    table.AddCell(item.Price.ToString("C")); // Format as currency
                }

                document.Add(table);
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph($"Invoice Total: {order.TotalAmount:C}")); // Format as currency
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("You can pay your bill by internet banking.\r\nOur account number is 02-3-3-33-3.\r\nPlease use your Client Id/Name in the reference field. "));

                document.Close();
            }

            return Path.Combine("invoices", invoiceFileName); // Return relative path for URL
        }

        [HttpPost("UpdateInvoiceStatus/{invoiceId}")]
        public async Task<IActionResult> UpdateInvoiceStatus(int invoiceId, [FromBody] string newStatus)
        {
            var invoice = await _dbContext.Invoice_Model.FindAsync(invoiceId);

            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            invoice.PaymentStatus = newStatus;
            _dbContext.Invoice_Model.Update(invoice);
            await _dbContext.SaveChangesAsync();

            return Ok("Invoice status updated successfully.");
        }

    }
}
   

