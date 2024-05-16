namespace Workshop.Models
{
    public class BookGenre
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? book { get; set; }
        public int GenreId { get; set; }
        public Genre? genre { get; set; }
    }
}
