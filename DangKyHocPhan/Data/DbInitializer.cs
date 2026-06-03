using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DangKyHocPhan.Models;

namespace DangKyHocPhan.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Tự động chạy Migration
            await context.Database.MigrateAsync();

            // 2. Seed Roles
            string[] roleNames = { "ADMIN", "STUDENT" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 3. Seed Admin User
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var createPowerUser = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "ADMIN");
                }
            }

            // 4. Seed Student User
            var studentEmail = "student@gmail.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);
            if (studentUser == null)
            {
                studentUser = new IdentityUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    EmailConfirmed = true
                };
                var createStudent = await userManager.CreateAsync(studentUser, "Student@123");
                if (createStudent.Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser, "STUDENT");
                }
            }

            // 5. Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Công nghệ thông tin" },
                    new Category { Name = "Kinh tế" },
                    new Category { Name = "Ngoại ngữ" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 6. Seed Courses (Cần ít nhất 7-8 học phần để test phân trang 5 học phần/trang)
            if (!await context.Courses.AnyAsync())
            {
                var itCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Công nghệ thông tin");
                var econCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Kinh tế");
                var langCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Ngoại ngữ");

                var courses = new List<Course>
                {
                    new Course
                    {
                        Name = "Lập trình Web ASP.NET Core",
                        Credits = 3,
                        Lecturer = "Nguyễn Văn A",
                        Image = "/images/Lap Trinh Web ASP.NET core.jpg",
                        CategoryId = itCategory?.Id ?? 1
                    },
                    new Course
                    {
                        Name = "Cơ sở dữ liệu SQL Server",
                        Credits = 3,
                        Lecturer = "Trần Thị B",
                        Image = "/images/SQL Sever.jpg",
                        CategoryId = itCategory?.Id ?? 1
                    },
                    new Course
                    {
                        Name = "Lập trình hướng đối tượng OOP",
                        Credits = 4,
                        Lecturer = "Phạm Văn C",
                        Image = "/images/OOP.webp",
                        CategoryId = itCategory?.Id ?? 1
                    },
                    new Course
                    {
                        Name = "Kinh tế vĩ mô",
                        Credits = 2,
                        Lecturer = "Lê Hoàng D",
                        Image = "/images/kinh-te-hoc-vi-mo.jpg",
                        CategoryId = econCategory?.Id ?? 2
                    },
                    new Course
                    {
                        Name = "Quản trị học",
                        Credits = 3,
                        Lecturer = "Hoàng Thị E",
                        Image = "/images/QuanTriHoc.jpg",
                        CategoryId = econCategory?.Id ?? 2
                    },
                    new Course
                    {
                        Name = "Tiếng Anh giao tiếp",
                        Credits = 2,
                        Lecturer = "John Smith",
                        Image = "/images/english.png",
                        CategoryId = langCategory?.Id ?? 3
                    },
                    new Course
                    {
                        Name = "Cấu trúc dữ liệu và giải thuật",
                        Credits = 4,
                        Lecturer = "Nguyễn Văn A",
                        Image = "/images/ctdl.png",
                        CategoryId = itCategory?.Id ?? 1
                    },
                    new Course
                    {
                        Name = "Phát triển ứng dụng di động",
                        Credits = 3,
                        Lecturer = "Trần Thị B",
                        Image = "/images/mobile.png",
                        CategoryId = itCategory?.Id ?? 1
                    }
                };

                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
            }
            else
            {
                // Cập nhật đường dẫn hình ảnh cho các học phần đã có sẵn trong CSDL
                var existingCourses = await context.Courses.ToListAsync();
                bool hasChanges = false;
                foreach (var course in existingCourses)
                {
                    if (course.Name == "Lập trình Web ASP.NET Core" && course.Image != "/images/Lap Trinh Web ASP.NET core.jpg")
                    {
                        course.Image = "/images/Lap Trinh Web ASP.NET core.jpg";
                        hasChanges = true;
                    }
                    else if (course.Name == "Cơ sở dữ liệu SQL Server" && course.Image != "/images/SQL Sever.jpg")
                    {
                        course.Image = "/images/SQL Sever.jpg";
                        hasChanges = true;
                    }
                    else if (course.Name == "Lập trình hướng đối tượng OOP" && course.Image != "/images/OOP.webp")
                    {
                        course.Image = "/images/OOP.webp";
                        hasChanges = true;
                    }
                    else if (course.Name == "Kinh tế vĩ mô" && course.Image != "/images/kinh-te-hoc-vi-mo.jpg")
                    {
                        course.Image = "/images/kinh-te-hoc-vi-mo.jpg";
                        hasChanges = true;
                    }
                    else if (course.Name == "Quản trị học" && course.Image != "/images/QuanTriHoc.jpg")
                    {
                        course.Image = "/images/QuanTriHoc.jpg";
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                {
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
