using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;

/// <summary>
/// Provides methods to initialise and seed the application's database context.
/// </summary>
public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContextInitialiser"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging database initialisation events.</param>
    /// <param name="context">The application's database context.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        IPasswordHasherService passwordHasher)
    {
        _logger = logger;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Ensures that the application's database is deleted and created anew.
    /// </summary>
    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
        }
    }

    /// <summary>
    /// Seeds the application's database with initial data if necessary.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    /// <summary>
    /// Attempts to seed the application's database with default data if necessary.
    /// </summary>
    public async Task TrySeedAsync()
    {
        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list" },
                    new TodoItem { Title = "Check off the first item" },
                    new TodoItem { Title = "Realise you've already done two things on the list!"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap" },
                }
            });

            await _context.SaveChangesAsync();
        }

        if (!_context.Users.Any())
        {
            var admin = new User
            {
                Username = "admin",
                NormalizedUsername = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirm = true,
                PhoneNumberConfirm = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                IsLocked = false,
                IsDeleted = false
            };
            admin.PasswordHash = _passwordHasher.HashPassword(admin, "Admin123!");

            var user = new User
            {
                Username = "user",
                NormalizedUsername = "USER",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirm = true,
                PhoneNumberConfirm = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                IsLocked = false,
                IsDeleted = false
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, "User123!");

            _context.Users.AddRange(admin, user);

            await _context.SaveChangesAsync();
        }
    }
}
