namespace PaymentGateway.Models
{
    public class ProductsXTransaction
    {
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public double Quantity { get; set; }
    }
}
