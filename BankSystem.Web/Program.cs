using BankSystem.Web.Data;
using BankSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankSystem.Web.Services;
using BankSystem.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILoanService, LoanService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roles = new[] { "Admin", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@bank.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FirstName = "System", LastName = "Admin" };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    var customerEmail = "customer@bank.com";
    var customerUser = await userManager.FindByEmailAsync(customerEmail);
    if (customerUser == null)
    {
        customerUser = new ApplicationUser { UserName = customerEmail, Email = customerEmail, FirstName = "Test", LastName = "Customer" };
        var result = await userManager.CreateAsync(customerUser, "Customer123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(customerUser, "Customer");
        }
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!dbContext.Customers.Any(c => c.UserId == customerUser.Id))
    {
        var customer = new Customer
        {
            UserId = customerUser.Id,
            FirstName = "Test",
            LastName = "Customer",
            DateOfBirth = new DateTime(1990, 1, 1),
            Address = "123 Mockingbird Lane"
        };
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();

        var account = new Account
        {
            CustomerId = customer.Id,
            AccountNumber = "CHK" + new Random().Next(1000000, 9999999).ToString(),
            AccountType = "Checking",
            Balance = 12500.50m
        };
        dbContext.Accounts.Add(account);

        var savingsAccount = new Account
        {
            CustomerId = customer.Id,
            AccountNumber = "SAV" + new Random().Next(1000000, 9999999).ToString(),
            AccountType = "Savings",
            Balance = 45000.00m
        };
        dbContext.Accounts.Add(savingsAccount);
        await dbContext.SaveChangesAsync();

        dbContext.Transactions.AddRange(
            new Transaction { AccountId = account.Id, Amount = 4500m, Type = "Deposit", Description = "Salary Direct Deposit" },
            new Transaction { AccountId = account.Id, Amount = 12.99m, Type = "Withdraw", Description = "Spotify Premium" },
            new Transaction { AccountId = account.Id, Amount = 150m, Type = "Withdraw", Description = "Grocery Store" },
            new Transaction { AccountId = savingsAccount.Id, Amount = 500m, Type = "Deposit", Description = "Monthly Transfer" }
        );
        
        dbContext.Cards.Add(new Card
        {
            AccountId = account.Id,
            CardNumber = "4291" + new Random().Next(1000, 9999) + new Random().Next(1000, 9999) + new Random().Next(1000, 9999),
            CVV = "123",
            ExpiryDate = DateTime.UtcNow.AddYears(3),
            CardType = "Debit"
        });

        await dbContext.SaveChangesAsync();
    }

    // Add more mock customers for the Admin Portal
    var additionalUsers = new[]
    {
        new { Email = "jane@bank.com", First = "Jane", Last = "Doe", Type = "Checking", Balance = 5400.25m },
        new { Email = "bob@bank.com", First = "Bob", Last = "Smith", Type = "Savings", Balance = 10200.00m }
    };

    foreach (var u in additionalUsers)
    {
        var uUser = await userManager.FindByEmailAsync(u.Email);
        if (uUser == null)
        {
            uUser = new ApplicationUser { UserName = u.Email, Email = u.Email, FirstName = u.First, LastName = u.Last };
            var result = await userManager.CreateAsync(uUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(uUser, "Customer");
                var c = new Customer
                {
                    UserId = uUser.Id,
                    FirstName = u.First,
                    LastName = u.Last,
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Address = "456 Fake Street"
                };
                dbContext.Customers.Add(c);
                await dbContext.SaveChangesAsync();

                var a = new Account
                {
                    CustomerId = c.Id,
                    AccountNumber = (u.Type == "Checking" ? "CHK" : "SAV") + new Random().Next(1000000, 9999999).ToString(),
                    AccountType = u.Type,
                    Balance = u.Balance
                };
                dbContext.Accounts.Add(a);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}

app.Run();
