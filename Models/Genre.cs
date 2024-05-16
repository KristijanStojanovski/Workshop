namespace Workshop.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string GenreName { get; set; }
        public ICollection<BookGenre>? bookGenres { get; set; }
    }
}
