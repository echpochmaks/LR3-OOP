using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FileStoreRepository : IStoreRepository
{
    private const string StoresFilePath = "stores.csv";

    public void CreateStore(Store store)
    {
        var stores = ReadStoresFromFile();

        stores.Add(store);

        WriteStoresToFile(stores);
    }

     public List<Store> GetAllStores()
    {
        return ReadStoresFromFile();
    }

     public List<Product> GetProductsByStoreCode(string storeCode)
    {
        var lines = File.ReadAllLines(StoresFilePath).Skip(1);
        var products = lines.Where(line => line.Split(',')[0] == storeCode)
                            .Select(line => line.Split(','))
                            .Select(parts => new Product
                            {
                                Name = parts[1],
                                Quantity = int.Parse(parts[3]), 
                                Price = decimal.Parse(parts[4])
                            })
                            .ToList();
        return products;
    }

    
    private List<Store> ReadStoresFromFile()
    {
        if (File.Exists(StoresFilePath))
        {
            var lines = File.ReadAllLines(StoresFilePath).Skip(1);
            return lines.Select(line => line.Split(','))
                        .Select(parts => new Store
                        {
                            Code = parts[0],
                            Name = parts[1],
                            Address = parts[2]
                        })
                        .ToList();
        }
        else
        {
            return new List<Store>();
        }
    }

    private void WriteStoresToFile(List<Store> stores)
    {
        var csvLines = new List<string> { "Code,Name,Address" };

        csvLines.AddRange(stores.Select(store =>
            $"{store.Code},{store.Name},{store.Address}"));

        File.WriteAllLines(StoresFilePath, csvLines);
    }
}
