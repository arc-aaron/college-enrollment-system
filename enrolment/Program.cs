using enrolment.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure database
builder.Services.AddDbContext<SchoolContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Initialize data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<SchoolContext>();
    DbInitializer.Initialize(context);
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error has occured creating the DB.");
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();