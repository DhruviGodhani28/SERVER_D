using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ExpenseManagement.Data;
using ExpenseManagement.Entity;

namespace EmployeeLeaveManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ExpenseDbContext _context;

        public UserController(ExpenseDbContext context)
        {
            _context = context;
        }

        [HttpGet("Getall")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllActiveUsers()
        {
            var activeUsers = await _context.users.Where(x => x.isactive == true).ToListAsync();
            if (activeUsers.Count == 0)
            {
                return NotFound(new { message = "No active User found." });
            }

            List<UserExpenseData> usersExpenseData = new List<UserExpenseData>();

            foreach (var user in activeUsers)
            {
                var totalExpense = await _context.expenses.Where(x => x.user_id == user.id).SumAsync(x => x.amount);
                var totalEarning = await _context.totalexpense.Where(x => x.userid == user.id).SumAsync(x => x.total_expense_amount);

                double totalSpend = Convert.ToDouble(totalExpense);
                double totalEarned = Convert.ToDouble(totalEarning);
                double difference = totalEarned - totalSpend;

                UserExpenseData userExpenseData = new UserExpenseData
                {
                    id = user.id,
                    name = user.name,
                    email = user.email,
                    password = user.password,
                    mobile_no = user.mobile_no,
                    isactive = user.isactive,
                    TotalEarning = difference,
                    TotalSpend = totalSpend,
                };

                usersExpenseData.Add(userExpenseData);
            }

            return Ok(new { message = "Active Users retrieved successfully.", data = usersExpenseData });
        }


        [HttpGet("Getbyid")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var users = await _context.users.FindAsync(id);

            if (users == null || !users.isactive)
            {
                return NotFound(new { message = "user not found or inactive." });
            }

            return Ok(new { message = "User retrieved successfully.", data = users });
        }

        [HttpPut("Update")]
        public async Task<ActionResult<User>> UpdateUser(User user)
        {
            var existingUser = await _context.users.FindAsync(user.id);

            if (existingUser == null)
            {
                return NotFound(new { message = "user not found." });
            }
            user.isactive = true;
            _context.Entry(existingUser).CurrentValues.SetValues(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw; // You may want to handle concurrency exceptions differently based on your application's requirements
            }

            return Ok(new { message = "User updated successfully.", data = existingUser });
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var users = await _context.users.FindAsync(id);
            if (users == null || !users.isactive)
            {
                return NotFound(new { message = "User not found or already inactive." });
            }

            users.isactive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User Deleted successfully.", data = id });
        }
    }
}
