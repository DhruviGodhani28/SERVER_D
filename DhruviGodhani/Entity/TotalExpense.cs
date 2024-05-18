using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManagement.Entity
{
    public class TotalExpense
    {
        [Key]
        public int  id { get; set; }
        [ForeignKey("User")]
        public int userid { get; set; }
        public double? total_expense_amount { get; set; }
    }
}
