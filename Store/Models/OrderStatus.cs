
namespace Store.Models
{
    public enum OrderStatus
    {
        Pending,        // Order started, waiting for payment/finalization
        Completed,      // Payment successful, order confirmed
        Cancelled,      // Order was cancelled
        Refunded,       // Order was refunded
        PaymentFailed   // Payment attempt failed
    }
}