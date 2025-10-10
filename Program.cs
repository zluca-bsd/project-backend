using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using project_backend;
using project_backend.Services;
using project_backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Register automapper service
builder.Services.AddAutoMapper(typeof(Program));

// Register services
builder.Services.AddScoped<IBooksService, BookService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Register the AppDbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register JwtSettings for DI injection
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Then do manual check
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (string.IsNullOrWhiteSpace(jwtSettings?.Secret) ||
    string.IsNullOrWhiteSpace(jwtSettings?.Issuer) ||
    string.IsNullOrWhiteSpace(jwtSettings?.Audience))
{
    throw new InvalidOperationException("JWT configuration is invalid.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Creates DB if not exists
    context.Database.EnsureCreated();
    context.SaveChanges();
}

app.Run();