using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [ForeignKey("usersId")]
        public string ?userId { get; set; }
        public string Email { get; set; }
        public string subject { get; set; }
        public string Message { get; set; }

    }
}
