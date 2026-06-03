using DangKyHocPhan.Data;
using DangKyHocPhan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DangKyHocPhan.Controllers
{
    [Route("Home/[action]")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("~/")]
        [Route("~/courses")]
        [Route("~/Home")]
        [Route("~/Home/Index")]
        public async Task<IActionResult> Index(int? page, string? searchString)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            if (pageNumber < 1) pageNumber = 1;

            // Thực hiện tìm kiếm theo tên học phần (Câu 8)
            var courseQuery = _context.Courses.Include(c => c.Category).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                courseQuery = courseQuery.Where(c => c.Name.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;

            var totalCourses = await courseQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCourses / pageSize);

            // Đảm bảo pageNumber không vượt quá totalPages (nếu có dữ liệu)
            if (totalPages > 0 && pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            var courses = await courseQuery
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy danh sách học phần sinh viên hiện tại đã đăng ký để hiển thị nút Đăng ký / Hủy đăng ký (Câu 6)
            var enrolledCourseIds = new List<int>();
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("STUDENT"))
            {
                var currentUserId = _userManager.GetUserId(User);
                if (currentUserId != null)
                {
                    enrolledCourseIds = await _context.Enrollments
                        .Where(e => e.UserId == currentUserId)
                        .Select(e => e.CourseId)
                        .ToListAsync();
                }
            }
            ViewBag.EnrolledCourseIds = enrolledCourseIds;

            var viewModel = new CourseListViewModel
            {
                Courses = courses,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
