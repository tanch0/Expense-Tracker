namespace TransactionApp.Repositories.Abstract
{
    public interface IRepository<T> where T : class
    {
        // Inserts a new entity of type T into the database.
        int Insert(T entity);
        // Inserts a range of entities of type T into the database.
        int InsertRange(IEnumerable<T> entities);
        // Updates an existing entity of type T in the database.
        int Update(T entity);
        // Deletes an existing entity of type T from the database.
        int Delete(T entity);
        // Deletes a range of entities of type T from the database.
        int DeleteRange(IEnumerable<T> entities);
        // Returns the total number of entities of type T in the database.
        int Count();
        // Returns a list of all entities of type T in the database.
        List<T> List();
        // Returns a list of entities of type T in the database, with optional paging parameters.
        List<T> List(int page, int pageSize);
        // Returns a list of all entities of type T in the database asynchronously.
        Task<List<T>> ListAsync();
        // Returns a list of entities of type T in the database asynchronously, with optional paging parameters.
        Task<List<T>> ListAsync(int page, int pageSize);
        // Returns an entity of type T by its primary key from the database.
        T? Find(int id);
        // Returns an entity of type T by its primary key from the database asynchronously.
        Task<T?> FindAsync(int id);
        // Returns an IEnumerable of entities of type T from the database.
        IEnumerable<T> GetEnumerable();
        // Returns an IQueryable of entities of type T from the database.
        IQueryable<T> GetQueryable();

    }
}
