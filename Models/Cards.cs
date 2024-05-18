using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Courses.Models
{
    public class Cards
    {
        [Key]
        public int CardId { get; set; }
        public string CardNumber { get; set; }

        public int CardValue { get; set; }

        [ForeignKey("userId")]
        public string ?userId { get; set; } // Nullable string for userId

    }
}
