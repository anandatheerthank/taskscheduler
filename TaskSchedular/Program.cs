using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using TaskSchedular.Data;
using TaskSchedular.Interface;
using TaskSchedular.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.--
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TaskContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DBConnection")
    ));

// Implement the code here to register the services.
builder.Services.AddScoped<ITasks, TaskRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

// Method override middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Task}/{action=Index}/{id?}");

app.Run();

