using System.ComponentModel.DataAnnotations;

namespace Workshop.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? YearPublished { get; set; }
        public int? NumPages { get; set; }
        public string? Publisher { get; set; }
        public string? FrontPage { get; set; }
        public string? DownloadUrl { get; set; }
        public int AuthorId { get; set; }
        public Author? author { get; set; }
        public ICollection<UserBooks>? UserBooks { get; set; }
        public ICollection<Review>? reviews { get; set; }
        public ICollection<BookGenre>? Genres { get; set; }
        public string FullTitle
        {
            get { return Title; }
        }
    }
}
