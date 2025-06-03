using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.Context;

public class DbInitializer
{
    private readonly FunewsManagementContext _context;
    private readonly IConfiguration _configuration;

    public DbInitializer(FunewsManagementContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        // Ensure database is created
        await _context.Database.EnsureCreatedAsync();

        // Check if we need to seed the default admin
        if (!await _context.SystemAccounts.AnyAsync())
        {
            var defaultAdmin = _configuration.GetSection("DefaultAdmin");
            var adminAccount = new SystemAccount
            {
                AccountId = 1,
                AccountEmail = defaultAdmin["Email"],
                AccountPassword = defaultAdmin["Password"],
                AccountRole = short.Parse(defaultAdmin["Role"] ?? "3"),
                AccountName = "System Administrator"
            };

            await _context.SystemAccounts.AddAsync(adminAccount);
            await _context.SaveChangesAsync();
        }
    }
} 