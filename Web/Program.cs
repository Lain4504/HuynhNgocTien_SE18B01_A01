using DAL.Context;
using DAL.Interfaces;
using DAL.Repositories;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.EntityFrameworkCore;
using DAL.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<FunewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Register services
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Register DbInitializer
builder.Services.AddScoped<DbInitializer>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FunewsManagementContext>();
        var initializer = services.GetRequiredService<DbInitializer>();
        await initializer.InitializeAsync();

        // Seed default admin if no accounts exist
        if (!await context.SystemAccounts.AnyAsync())
        {
            var defaultAdmin = builder.Configuration.GetSection("DefaultAdmin");
            var adminAccount = new SystemAccount
            {
                AccountId = (short)(await context.SystemAccounts.MaxAsync(x => (int?)x.AccountId) ?? 0 + 1),
                AccountEmail = defaultAdmin["Email"],
                AccountPassword = defaultAdmin["Password"],
                AccountRole = short.Parse(defaultAdmin["Role"] ?? "3"),
                AccountName = "System Administrator"
            };

            await context.SystemAccounts.AddAsync(adminAccount);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
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

// Add session middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
