using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Lessons
    {
        [Key]
        public int LessonsId { get; set; }
        public string LessonsName { get; set; }
        public string LessonsDescription { get; set;}

      [ForeignKey("courseId ")]

        public int courseId { get; set; }
    
        public string URL {  get; set; }


    }
}

