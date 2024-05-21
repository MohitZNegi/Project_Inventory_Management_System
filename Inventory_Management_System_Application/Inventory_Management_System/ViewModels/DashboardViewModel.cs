using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{



    public class DashboardViewModel
    {
        public PurchaseOrderStatusViewModel PurchaseOrderStatus { get; set; }
        public IEnumerable<DeliveryHistoryReportViewModel> DeliveryHistoryReports { get; set; }
        public IEnumerable<SupplierViewModel> SupplierViewModels { get; set; }
    }
}



