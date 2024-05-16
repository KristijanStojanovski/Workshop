using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Workshop.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int bookId { get; set; }
        public Book? Book { get; set; }
        [DisplayName("User")]
        public string AppUser { get; set; }
        public string Comment { get; set; }
        [Range(1,10)]
        public int? rating { get; set; }
    }
}
