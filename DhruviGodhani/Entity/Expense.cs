using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExpenseManagement.Entity
{
    public class Expense
    {
        [Key]
        public int id { get; set; } 

        [Required]
        [ForeignKey("User")]
        public int user_id { get; set; }
        public string name { get; set; }

        public DateTime date { get; set; }
        public decimal amount { get; set; } 
    }

    public class ShowExpense
    {
        public double? totalEarning { get; set; }
        public double? totalSpend { get; set; }
    }
}
