
using System;
using System.Collections.Generic;

namespace MoneyTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}