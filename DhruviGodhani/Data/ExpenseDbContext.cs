using ExpenseManagement.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagement.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> users { get; set; }
        public DbSet<Expense> expenses { get; set; }
        public DbSet<TotalExpense> totalexpense { get; set; }
    }
}
