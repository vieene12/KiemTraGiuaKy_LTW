using System.Collections.Generic;

namespace DangKyHocPhan.Models
{
    public class CourseListViewModel
    {
        public IEnumerable<Course> Courses { get; set; } = new List<Course>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
