using System;
using MoneyTracker.Models;

namespace MoneyTracker.DTOs
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionMethod Method { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}