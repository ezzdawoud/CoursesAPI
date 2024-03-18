using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Comments
    {
        [Key]
        public int CommentsId { get; set; }
       [ForeignKey("LessonsId")]
       public int LessonsId { get; set; }
      
        [ForeignKey("userId")]
        public string userId {  get; set; }
      
        public DateTime date {  get; set; }
    }
}
