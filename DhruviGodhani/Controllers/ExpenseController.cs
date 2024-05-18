using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManagement.Data;
using ExpenseManagement.Entity;

namespace EmployeeLeaveManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseDbContext _context;

        public ExpenseController(ExpenseDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetAllExpenses()
        {
            int id = int.Parse(User.Identity.Name);
            var expenses = await _context.expenses.Where(x => x.user_id == id).ToListAsync();
            return Ok(new { message = "All expenses retrieved successfully.", data = expenses });
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Expense>> GetExpenseById(int id)
        {
            var expense = await _context.expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound(new { message = "Expense not found." });
            }

            return Ok(new { message = "Expense retrieved successfully.", data = expense });
        }

        [HttpPost("Create")]
        public async Task<ActionResult<Expense>> CreateExpense(Expense expense)
        {
            int userid = int.Parse(User.Identity.Name);
            expense.user_id = userid;
            _context.expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense created successfully.", data = expense });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateExpense(Expense expense)
        {
            int id = expense.id;
            int userid = int.Parse(User.Identity.Name);
            expense.user_id = userid;
            var existingExpense = await _context.expenses.FindAsync(id);
            if (existingExpense == null)
            {
                return NotFound(new { message = "Expense not found." });
            }

            _context.Entry(existingExpense).CurrentValues.SetValues(expense);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
                {
                    return NotFound(new { message = "Expense not found." });
                }
                else
                {
                    throw; // You may want to handle concurrency exceptions differently based on your application's requirements
                }
            }

            return Ok(new { message = "Expense updated successfully.", data = expense });
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound(new { message = "Expense not found." });
            }

            _context.expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense deleted successfully.", data = id });
        }

        [HttpPost("TotalExpense")]
        public async Task<ActionResult<Expense>> TotalExpense(TotalExpense totalExpense)
        {
            
            int userid = int.Parse(User.Identity.Name);
            totalExpense.userid = userid;
            _context.totalexpense.Add(totalExpense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense created successfully.", data = totalExpense });
        }

        [HttpGet("ShowTotalExpense")]
        public async Task<ActionResult<ShowExpense>> ShowTotalExpense()
        {
            int userId = int.Parse(User.Identity.Name);
            var totalExpense = await _context.expenses.Where(x => x.user_id == userId).SumAsync(x => x.amount);

            var totalEarning = await _context.totalexpense.Where(x => x.userid == userId).SumAsync(x => x.total_expense_amount);

            double totalSpend = Convert.ToDouble(totalExpense);
            double totalEarned = Convert.ToDouble(totalEarning);
            double difference = totalEarned - totalSpend;

            ShowExpense expenseData = new ShowExpense
            {
                totalEarning = difference,
                totalSpend = totalSpend,
            };

            if (expenseData == null)
            {
                return NotFound(new { message = "Expense data not found." });
            }

            return Ok(new { message = "Expense data retrieved successfully.", data = expenseData });
        }


        private bool ExpenseExists(int id)
        {
            return _context.expenses.Any(e => e.id == id);
        }
    }
}
