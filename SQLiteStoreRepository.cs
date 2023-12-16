public class SQLiteStoreRepository : IStoreRepository
{
    private SQLiteDbContext _dbContext;

    public SQLiteStoreRepository(SQLiteDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    public void CreateStore(Store store)
    {
        _dbContext.Stores.Add(store);
        _dbContext.SaveChanges();
    }

    public List<Store> GetAllStores()
    {
        return _dbContext.Stores.ToList();
    }

    public List<Product> GetProductsByStoreCode(string storeCode)
    {
        return _dbContext.Products.Where(p => p.StoreCode == storeCode).ToList();
    }

    public Store GetStoreByCode(string storeCode)
    {
        return _dbContext.Stores.FirstOrDefault(s => s.Code == storeCode);
    }

    public void UpdateStore(Store store)
    {
        _dbContext.Stores.Update(store);
        _dbContext.SaveChanges();
    }
}