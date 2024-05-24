using System.ComponentModel.DataAnnotations;

namespace Courses.Models
{
    public class Reactions
    {
        [Key]
        public int reactionId { get; set; }
        public string userId { get; set; }
        public int ?commentId {  get; set; }
        public int ?SubCommentsId {  get; set; }
        public bool reaction {  get; set; }
    }
}
