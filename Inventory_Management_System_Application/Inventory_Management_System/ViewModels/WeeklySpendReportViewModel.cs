using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.ViewModels
{

    public class WeeklySpendReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<WeeklySpendDataRange> WeeklySpendData { get; set; }
    }

    public class WeeklySpendDataRange
    {
        public DateTime WeekStartDate { get; set; }
        public DateTime WeekEndDate { get; set; }
        public float TotalSpend { get; set; }
    }
}


