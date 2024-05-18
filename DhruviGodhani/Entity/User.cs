using ExpenseManagement.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class User
{
    [Key]
    public int id { get; set; } // Primary key

    public string name { get; set; }

    public string email { get; set; }

    public string password { get; set; }

    public string mobile_no { get; set; }

    public bool isactive { get; set; }
}

public class Login
{
    public string email { get; set; }
    public string password { get; set; }
}


public class UserExpenseData
{
    //public User User { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string mobile_no { get; set; }
    public bool isactive { get; set; }
    public double TotalEarning { get; set; }
    public double TotalSpend { get; set; }
}