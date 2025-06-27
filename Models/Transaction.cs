
using System;

namespace MoneyTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionMethod Method { get; set; }
        public CurrencyType Currency { get; set; } = CurrencyType.USD;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        
        public User User { get; set; }
        public Category Category { get; set; }
    }
}