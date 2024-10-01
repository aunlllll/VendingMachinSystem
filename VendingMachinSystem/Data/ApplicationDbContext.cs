using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VendingMachinSystem.Models;

namespace VendingMachinSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product>  Products { get; set; }
        public DbSet<ApplicationUser>  ApplicationUsers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}   
