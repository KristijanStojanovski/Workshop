using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Workshop.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("BirthDay")]
        public DateTime? BirthDate { get; set; }
        public string? Nationality { get; set; }
        public string? Gender { get; set; }
        public ICollection<Book>? books { get; set; }
        [DisplayName("Author")]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}
