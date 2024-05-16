namespace Workshop.Models
{
    public class UserBooks
    {
        public int Id { get; set; }
        public string AppUser { get; set; }
        public int BookId { get; set; }
        public Book? books { get; set; }
    }
}
