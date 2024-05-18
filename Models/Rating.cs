using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Rating
    {
        [Key]
        public int RatingId {  get; set; }
        public int rating {  get; set; }
        [ForeignKey("usersId")]
        public string usersId {  get; set; }
        [ForeignKey("courseId")]

        public int courseId {  get; set; }
    }
}
