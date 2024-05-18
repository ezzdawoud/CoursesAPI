using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Courses
    {
        [Key]
        public int courseId { get; set; }
        public string CouresName { get; set; }
        public string CouresDescription { get; set;}
        public string CouresType { get; set; }
        public string CoursesCatagory { get; set; }
        public string CouresLanguage { get; set; }
        public int CouresValue {  get; set; }   
        [ForeignKey("UsersId")]
        public string UsersId { get; set; }
        public DateTime Date {  get; set; }
        public int numberOfLessons {  get; set; }
        public string ?Pictures {  get; set; }
        public int Rating {  get; set; }

        public static explicit operator int(Courses v)
        {
            throw new NotImplementedException();
        }
    }
}
