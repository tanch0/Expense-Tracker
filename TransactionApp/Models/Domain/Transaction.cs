namespace TransactionApp.Models.Domain
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        // User-specific information
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Transaction details
        public string TransactionName { get; set; }
        public bool TransactionType { get; set; }
        public string TransactionDescription { get; set; }
        public int TransactionAmount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime EditedDateTime { get; set; }
    }
}
