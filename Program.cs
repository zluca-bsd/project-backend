var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var app = builder.Build();

// Resister services

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();