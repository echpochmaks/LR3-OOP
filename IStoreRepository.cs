public interface IStoreRepository
{
    void CreateStore(Store store);
    List<Product> GetProductsByStoreCode(string storeCode);
    List<Store> GetAllStores();
}
