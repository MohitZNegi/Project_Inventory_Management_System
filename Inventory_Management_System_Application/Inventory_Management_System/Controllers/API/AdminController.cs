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

namespace Inventory_Management_System.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
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

            var invoice = GenerateInvoice(order, orderItems);
           // SendInvoiceToClient(order.User.Email, invoice);

            return Ok("Order delivery confirmed. Invoice sent to the client.");
        }

        private MemoryStream GenerateInvoice(Order order, List<OrderItem> orderItems)
        {
            var memoryStream = new MemoryStream();
            var document = new Document();
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            var logo = iTextSharp.text.Image.GetInstance("path_to_logo.png"); // Update with your logo path
            logo.ScalePercent(50f);
            document.Add(logo);

            document.Add(new Paragraph($"Invoice for Order #{order.OrderId}", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
            document.Add(new Paragraph($"Order Date: {order.OrderDate:MM/dd/yyyy}"));
            document.Add(new Paragraph($"Due Date: {order.OrderDate.AddDays(30):MM/dd/yyyy}")); // Assuming a 30-day payment period
            document.Add(new Paragraph($"Client: {order.User.UserName}"));
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
            document.Add(new Paragraph($"Total Amount: {order.TotalAmount:C}")); // Format as currency

            document.Close();
            memoryStream.Position = 0;
            return memoryStream;
        }
      

    }
}
   

