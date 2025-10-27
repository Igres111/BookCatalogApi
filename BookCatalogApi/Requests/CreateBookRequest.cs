using System.ComponentModel.DataAnnotations;

namespace BookCatalogApi.Dtos
{
    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int PublicationYear { get; set; }
    }
}
