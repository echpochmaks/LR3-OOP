using System.Collections.Generic;

public interface IProductService
{
    void CreateProduct(string productName);
    List<Product> GetProductsByStore(string storeCode);
    // Другие методы...
}

public interface IProductRepository
{
    void CreateProduct(Product product);
    Product GetProductByName(string productName, string storeCode);
    List<Product> GetProductsByStoreCode(string storeCode);
    void UpdateProduct(Product product);
    void UpdateProducts(string storeCode, List<Product> products);
    List<Product> GetProductsByProductName(string productName);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public void CreateProduct(string productName)
    {
        Product newProduct = new Product
        {
            Name = productName
            // Другие свойства товара могут быть установлены по умолчанию
        };

        _productRepository.CreateProduct(newProduct);
    }

    // Добавим метод для получения списка товаров в магазине
    public List<Product> GetProductsByStore(string storeCode)
    {
        return _productRepository.GetProductsByStoreCode(storeCode);
    }
}
