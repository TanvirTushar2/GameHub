using System;

namespace GameHub.Models
{
    public class Game
    {
        public int GameID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int GenreID { get; set; }
        public string GenreName { get; set; }
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsActive { get; set; }
    }
}
