

using MoneyTracker.Models;

namespace MoneyTracker.DTOs
{
    public class UpdateTransactionRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionMethod Method { get; set; }
    }
}