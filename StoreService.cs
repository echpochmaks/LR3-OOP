using System;
using System.Collections.Generic;
using System.Linq;

namespace LR3
{
    public class ProductQuantity
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public interface IStoreService
    {
        void CreateStore(string storeCode, string storeName, string address);
        void StockProducts(string storeCode, List<ProductQuantity> products);
        List<Product> GetProductsByStore(string storeCode);
        List<Product> GetAffordableProducts(string storeCode, decimal budget);
        decimal PurchaseProductsInStore(string storeCode, List<ProductQuantity> productsToBuy);
        string FindCheapestStoreForProduct(string productName);
        string FindCheapestStoreForProductSet(List<ProductQuantity> productsToBuy);
    }

    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IProductRepository _productRepository;

        public StoreService(IStoreRepository storeRepository, IProductRepository productRepository)
        {
            _storeRepository = storeRepository;
            _productRepository = productRepository;
        }

        public List<Product> GetProductsByStore(string storeCode)
        {
            return _productRepository.GetProductsByStoreCode(storeCode);
        }

        public void CreateStore(string storeCode, string storeName, string address)
        {
            Store newStore = new Store
            {
                Code = storeCode,
                Name = storeName,
                Address = address
            };

            _storeRepository.CreateStore(newStore);
        }


    
        public List<Product> GetAffordableProducts(string storeCode, decimal budget)
        {
            List<Product> allProducts = _productRepository.GetProductsByStoreCode(storeCode);
            List<Product> affordableProducts = new List<Product>();

            foreach (var product in allProducts)
            {
                decimal unitPrice = product.Price;

                if (unitPrice > 0)
                {
                    int affordableQuantity = (int)Math.Floor(budget / unitPrice);

                    if (affordableQuantity > 0)
                    {
                        affordableProducts.Add(new Product
                        {
                            Name = product.Name,
                            Quantity = affordableQuantity,
                            Price = unitPrice
                        });
                    }
                }
            }

            return affordableProducts;
        }


        public string FindCheapestStoreForProduct(string productName)
        {
            Dictionary<string, decimal> storeTotalCosts = new Dictionary<string, decimal>();
            List<Product> availableProducts = _productRepository.GetProductsByProductName(productName);

            if (!availableProducts.Any())
            {
                Console.WriteLine($"Товар '{productName}' не найден в магазинах.");
                return null;
            }

            foreach (var availableProduct in availableProducts)
            {
                decimal totalCost = availableProduct.Quantity * availableProduct.Price;

                if (!storeTotalCosts.ContainsKey(availableProduct.StoreCode))
                {
                    storeTotalCosts[availableProduct.StoreCode] = totalCost;
                }
                else
                {
                    storeTotalCosts[availableProduct.StoreCode] += totalCost;
                }
            }

            if (storeTotalCosts.Count > 0)
            {
                string cheapestStore = storeTotalCosts.OrderBy(kv => kv.Value).First().Key;
                return cheapestStore;
            }

            return null;
        }

        public string FindCheapestStoreForProductSet(List<ProductQuantity> productsToBuy)
        {
            Dictionary<string, decimal> storeTotalCosts = new Dictionary<string, decimal>();

            foreach (var productToBuy in productsToBuy)
            {
                List<Product> availableProducts = _productRepository.GetProductsByProductName(productToBuy.ProductName);

                if (!availableProducts.Any())
                {
                    Console.WriteLine($"Товар '{productToBuy.ProductName}' не найден в магазинах.");
                    return null;
                }

                foreach (var availableProduct in availableProducts)
                {
                    decimal totalCost = productToBuy.Quantity * availableProduct.Price;

                    if (!storeTotalCosts.ContainsKey(availableProduct.StoreCode))
                    {
                        storeTotalCosts[availableProduct.StoreCode] = totalCost;
                    }
                    else
                    {
                        storeTotalCosts[availableProduct.StoreCode] += totalCost;
                    }
                }
            }

            if (storeTotalCosts.Count > 0)
            {
                string cheapestStore = storeTotalCosts.OrderBy(kv => kv.Value).First().Key;
                return cheapestStore;
            }

            return null;
        }


        public decimal PurchaseProductsInStore(string storeCode, List<ProductQuantity> productsToBuy)
        {
            List<Product> availableProducts = _productRepository.GetProductsByStoreCode(storeCode);
            decimal totalCost = 0.0m;

            foreach (var productToBuy in productsToBuy)
            {
                Product availableProduct = availableProducts.FirstOrDefault(p => p.Name == productToBuy.ProductName);

                if (availableProduct == null || availableProduct.Quantity < productToBuy.Quantity)
                {
                    return -1;
                }
                totalCost += productToBuy.Quantity * availableProduct.Price;
                availableProduct.Quantity -= productToBuy.Quantity;
            }
            _productRepository.UpdateProducts(storeCode, availableProducts);

            return totalCost;
        }

        public void StockProducts(string storeCode, List<ProductQuantity> products)
        {
            foreach (var productQuantity in products)
            {
                // Проверка существующего товара
                Product existingProduct = _productRepository.GetProductByName(productQuantity.ProductName, storeCode);

                if (existingProduct != null)
                {
                    // Товар уже существует, увеличиваем количество
                    existingProduct.Quantity += productQuantity.Quantity;
                    existingProduct.Price = productQuantity.Price;
                    _productRepository.UpdateProduct(existingProduct);
                }
                else
                {
                    // Товар не существует, создаем новый
                    Product newProduct = new Product
                    {
                        Name = productQuantity.ProductName,
                        StoreCode = storeCode,
                        Quantity = productQuantity.Quantity,
                        Price = productQuantity.Price
                    };

                    _productRepository.CreateProduct(newProduct);
                }
            }
        }
    }
}