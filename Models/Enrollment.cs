using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        [ForeignKey("UserId")]
        public string UserId {  get; set; }
  
        [ForeignKey("CouresId")]
         public int ?CouresId { get; set; }
        public int enrollmentValue {  get; set; }
        public string teacherId {  get; set; }
        public DateTime EnrollmentDate { get; set; } // Added date property


    }
}
