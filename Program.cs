using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using project_backend;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Register the AppDbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Creates DB if not exists
    context.Database.EnsureCreated();

    // context.Products.Add(new Product("Mouse", 30f, "computer-accessory"));
    // context.Products.Add(new Product("Keyboard", 50f, "computer-accessory"));
    // context.Products.Add(new Product("Monitor", 150f, "computer-accessory"));

    // Product product1 = new Product("Headphones", 10f, "computer-accessory");
    // Customer customer1 = new Customer("Maxim", "maxim@mail.com", "10/10/2000");

    // context.Products.Add(product1);
    // context.Orders.Add(new Order(customer1.Id, product1.Id));
    
    context.SaveChanges();
}


app.Run();

// using var context = new AppDbContext();

// context.Customers.Add(new Customer("Alessandro", "alessandro123@mail.com", "10/10/2000"));

// context.SaveChanges();


