namespace GameHub.Models
{
    /// <summary>One line in the shopping cart.</summary>
    public class CartItem
    {
        public Game Game { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get { return Game.Price * Quantity; } }
    }
}
