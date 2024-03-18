using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        [ForeignKey("userId")]
        public string UserId {  get; set; }
  
        [ForeignKey("CouresId")]
         public int CouresId { get; set; }
    


    }
}
