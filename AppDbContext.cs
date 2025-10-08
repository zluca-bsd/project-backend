using Microsoft.EntityFrameworkCore;
using project_backend.Models.BookModels;
using project_backend.Models.CustomerModels;
using project_backend.Models.OrderModels;

namespace project_backend
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}