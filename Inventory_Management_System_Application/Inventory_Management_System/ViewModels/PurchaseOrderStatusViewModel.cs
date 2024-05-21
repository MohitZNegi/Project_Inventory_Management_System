using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    public class PurchaseOrderStatusViewModel
    {
        public int DeliveredCount { get; set; }
        public int PendingCount { get; set; }
        public int OnTheWayCount { get; set; }
    }
}


