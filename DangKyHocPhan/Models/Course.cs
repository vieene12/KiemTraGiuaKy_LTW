using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DangKyHocPhan.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên học phần không được để trống")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Số tín chỉ không được để trống")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Tên giảng viên không được để trống")]
        [StringLength(100)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục học phần")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
