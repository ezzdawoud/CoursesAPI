using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class SubComments

    {
        [Key]
        public int SubCommentsId { get; set; }
        [ForeignKey("LessonsId")]
        public int LessonsId { get; set; }
        [ForeignKey("CommentsId")]
        public int CommentsId {  get; set; }
        [ForeignKey("userId")]

        public string userId {  get; set; }
        public string subComments {  get; set; }
        public int like { get; set; }
        public int dislike { get; set; }


    }
}
