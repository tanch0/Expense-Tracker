using TransactionApp.Models.Domain;
using TransactionApp.Repositories.Abstract;

namespace TransactionApp.Repositories.Implementation
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
