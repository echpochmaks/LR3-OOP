using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FileProductRepository : IProductRepository
{
    private const string ProductsFilePath = "products.csv";
    private List<Product> _products; // Initialize the _products list

    public FileProductRepository()
    {
        _products = ReadProductsFromFile(); // Initialize _products in the constructor
    }

    public void CreateProduct(Product product)
    {
        _products.Add(product); // Use _products
        WriteProductsToFile(_products);
    }

    public Product GetProductByName(string productName, string storeCode)
    {
        return _products.FirstOrDefault(p => p.Name == productName && p.StoreCode == storeCode);
    }

    public List<Product> GetProductsByStoreCode(string storeCode)
    {
        return _products.Where(p => p.StoreCode == storeCode).ToList();
    }

    public void UpdateProduct(Product product)
    {
        Product existingProduct = _products.FirstOrDefault(p =>
            p.Name == product.Name && p.StoreCode == product.StoreCode);

        if (existingProduct != null)
        {
            existingProduct.Quantity = product.Quantity;
            existingProduct.Price = product.Price;
        }

        WriteProductsToFile(_products);
    }

    public List<Product> GetProductsByProductName(string productName)
    {
        return _products.Where(p => p.Name.Contains(productName)).ToList();
    }

    private List<Product> ReadProductsFromFile()
    {
        if (File.Exists(ProductsFilePath))
        {
            var lines = File.ReadAllLines(ProductsFilePath).Skip(1);
            return lines.Select(line => line.Split(','))
                        .Select(parts => new Product
                        {
                            Name = parts[0],
                            StoreCode = parts[1],
                            Quantity = int.Parse(parts[2]),
                            Price = parts.Length > 3 ? decimal.Parse(parts[3]) : 0.0m
                        })
                        .ToList();
        }
        else
        {
            return new List<Product>();
        }
    }

    private void WriteProductsToFile(List<Product> products)
    {
        var csvLines = new List<string> { "Name,StoreCode,Quantity,Price" };

        csvLines.AddRange(products.Select(product =>
            $"{product.Name},{product.StoreCode},{product.Quantity},{product.Price}"));

        File.WriteAllLines(ProductsFilePath, csvLines);
    }

    public void UpdateProducts(string storeCode, List<Product> products)
    {
        foreach (var updatedProduct in products)
        {
            Product existingProduct = _products.FirstOrDefault(p =>
                p.Name == updatedProduct.Name && p.StoreCode == storeCode);

            if (existingProduct != null)
            {
                existingProduct.Quantity = updatedProduct.Quantity;
                existingProduct.Price = updatedProduct.Price;
            }
        }

        WriteProductsToFile(_products);
    }
}
