using Microsoft.AspNetCore.Identity;
using LabBookingSystem.Models;

namespace LabBookingSystem.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        
        string[] roles = { "Admin", "Staff", "Student" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        
        if (await userManager.FindByEmailAsync("admin@university.ac") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@university.ac",
                Email = "admin@university.ac",
                FullName = "IT Administrator",
                Department = "IT Services",
                StudentStaffId = "IT001",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        
        if (await userManager.FindByEmailAsync("staff@university.ac") == null)
        {
            var staff = new ApplicationUser
            {
                UserName = "staff@university.ac",
                Email = "staff@university.ac",
                FullName = "Jane Smith",
                Department = "Computer Science",
                StudentStaffId = "ST001",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(staff, "Staff@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(staff, "Staff");
        }

        
        if (await userManager.FindByEmailAsync("student@university.ac") == null)
        {
            var student = new ApplicationUser
            {
                UserName = "student@university.ac",
                Email = "student@university.ac",
                FullName = "John Doe",
                Department = "Computer Science",
                StudentStaffId = "STU001",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(student, "Student@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(student, "Student");
        }

        
        if (!context.Resources.Any())
        {
            context.Resources.AddRange(
                new Resource
                {
                    Name = "Lab A – Windows Suite",
                    Type = ResourceType.ComputerLab,
                    Location = "Block 1, Room 101",
                    Description = "30-seat Windows 11 lab with dual monitors and Office 365.",
                    Capacity = 30
                },
                new Resource
                {
                    Name = "Lab B – Linux Suite",
                    Type = ResourceType.ComputerLab,
                    Location = "Block 1, Room 102",
                    Description = "25-seat Ubuntu lab, ideal for programming and networking modules.",
                    Capacity = 25
                },
                new Resource
                {
                    Name = "Lab C – Mac Creative Suite",
                    Type = ResourceType.ComputerLab,
                    Location = "Block 2, Room 201",
                    Description = "20-seat Apple Mac lab with Adobe Creative Cloud and Final Cut Pro.",
                    Capacity = 20
                },
                new Resource
                {
                    Name = "Workstation WS-01",
                    Type = ResourceType.Workstation,
                    Location = "Block 1, Room 101",
                    Description = "High-performance PC for video editing: i9, RTX 4090, 64 GB RAM.",
                    Capacity = 1
                },
                new Resource
                {
                    Name = "Workstation WS-02",
                    Type = ResourceType.Workstation,
                    Location = "Block 1, Room 101",
                    Description = "High-performance PC for 3D rendering and simulation workloads.",
                    Capacity = 1
                },
                new Resource
                {
                    Name = "Projector PRJ-01",
                    Type = ResourceType.Projector,
                    Location = "IT Store – Level 1",
                    Description = "Epson 4K portable laser projector with HDMI, USB-C, and wireless.",
                    Capacity = 1
                },
                new Resource
                {
                    Name = "Projector PRJ-02",
                    Type = ResourceType.Projector,
                    Location = "IT Store – Level 1",
                    Description = "BenQ Full HD short-throw portable projector.",
                    Capacity = 1
                },
                new Resource
                {
                    Name = "AV Kit AV-01",
                    Type = ResourceType.AVEquipment,
                    Location = "IT Store – Level 1",
                    Description = "Complete AV kit: Bluetooth speaker, wireless microphone, HDMI hub, and extension cable.",
                    Capacity = 1
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
