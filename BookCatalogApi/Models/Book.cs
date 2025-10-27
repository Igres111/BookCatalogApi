﻿namespace BookCatalogApi.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorID { get; set; }   
        public int PublicationYear { get; set; }
    }
}