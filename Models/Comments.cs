using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Comments
    {
        [Key]
        public int CommentsId { get; set; }
        public string comments {  get; set; }
       [ForeignKey("LessonsId")]
       public int LessonsId { get; set; }
        public Lessons Lessons { get; set; }
      
        [ForeignKey("UsersId")]
        public string UsersId {  get; set; }
      
        public DateTime date {  get; set; } = DateTime.Now;
        public int like {  get; set; }
        public int dislike {  get; set; }
        public List<SubComments>?SubCommentsList { get; set; }

    }
}
