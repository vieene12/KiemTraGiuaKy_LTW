using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DangKyHocPhan.Data;
using DangKyHocPhan.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DangKyHocPhan.Controllers
{
    [Authorize(Roles = "STUDENT")]
    [Route("enroll")]
    public class EnrollController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EnrollController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: enroll/add/5
        [HttpPost("add/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollCourse(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                return NotFound("Không tìm thấy học phần.");
            }

            var isAlreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (!isAlreadyEnrolled)
            {
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = courseId,
                    EnrollDate = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }

            // Redirect về trang trước hoặc trang chủ
            string? referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: enroll/remove/5
        [HttpPost("remove/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnenrollCourse(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            string? referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: enroll/my-courses
        [HttpGet("my-courses")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var enrolledCourses = await _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c!.Category)
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.EnrollDate)
                .Select(e => e.Course)
                .ToListAsync();

            return View(enrolledCourses);
        }
    }
}
