using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Courses.Models
{
    public class Cards
    {
        [Key]
        public int CardId { get; set; }
        public int CardValue { get; set; }
        [ForeignKey("User")]
         public string ?userId {  get; set; }


    }
}
