using System;
using System.Collections.Generic;
using LR3;

class Program
{
    private static IStoreRepository storeRepository;
    private static IProductRepository productRepository;
    private static IStoreService storeService;

    static void Main()
    {   
        InitializeRepositories();
        InitializeServices();
        
        storeService = CreateStoreService("sqlite", storeRepository, productRepository);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Создать новый магазин");
            Console.WriteLine("2. Добавить товар в магазин");
            Console.WriteLine("3. Просмотреть товары в магазине");
            Console.WriteLine("4. Добавить товары в магазин");
            Console.WriteLine("5. Найти самый дешевый магазин для товара");
            Console.WriteLine("6. Получить доступные товары в магазине по бюджету");
            Console.WriteLine("7. Совершить покупку в магазине");
            Console.WriteLine("8. Найти самый дешевый магазин для партии товаров");
            Console.WriteLine("9. Просмотреть информацию обо всех магазинах");
            Console.WriteLine("10. Выйти");

            Console.Write("Выберите действие (1-10): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                default:
                    Console.WriteLine("Неверный ввод. Пожалуйста, выберите от 1 до 4.");
                    break;
                case "1":
                    CreateNewStore();
                    break;
                case "2":
                    AddProductToStore();
                    break;
                case "3":
                    ViewProductsInStore();
                    break;                      
                case "4":
                    ImportProductsToStore();
                    break;
                case "5":
                    FindCheapestStoreForProduct();
                    break;
                case "6":
                    GetAffordableProductsInStore();
                    break;
                case "7":
                    PurchaseProductsInStore();
                    break;
                case "8":
                    FindCheapestStoreForProductSet();
                    break;
                case "9":
                    DisplayAllStores();
                    break;
                case "10":
                    Environment.Exit(0);
                    break;
            }

            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }

    private static void InitializeRepositories()
    {
        storeRepository = new FileStoreRepository();
        productRepository = new FileProductRepository();
    }

    private static void InitializeServices()
    {
        storeService = new StoreService(storeRepository, productRepository);
    }

    private static void CreateNewStore()
    {
        Console.Write("Введите код нового магазина: ");
        string storeCode = Console.ReadLine();

        Console.Write("Введите имя нового магазина: ");
        string storeName = Console.ReadLine();

        Console.Write("Введите адрес нового магазина: ");
        string storeAddress = Console.ReadLine();

        storeService.CreateStore(storeCode, storeName, storeAddress);

        Console.WriteLine("Магазин успешно создан!");
    }

    private static void DisplayAllStores()
    {
        Console.WriteLine("Информация о всех магазинах:");

        var stores = storeRepository.GetAllStores();

        if (stores.Count > 0)
        {
            foreach (var store in stores)
            {
                Console.WriteLine($"Код: {store.Code}, Название: {store.Name}, Адрес: {store.Address}");
            }
        }
        else
        {
            Console.WriteLine("Магазины не найдены.");
        }
    }

    private static void AddProductToStore()
    {
        Console.Write("Введите код магазина: ");
        string storeCode = Console.ReadLine();

        Console.Write("Введите имя товара: ");
        string productName = Console.ReadLine();

        Console.Write("Введите количество товара: ");
        int quantity = int.Parse(Console.ReadLine());

        Console.Write("Введите цену товара: ");
        decimal price = decimal.Parse(Console.ReadLine());

        List<ProductQuantity> products = new List<ProductQuantity>
        {
            new ProductQuantity { ProductName = productName, Quantity = quantity, Price = price }
        };

        storeService.StockProducts(storeCode, products);

        Console.WriteLine("Товар успешно добавлен в магазин!");
    }

    private static void ViewProductsInStore()
    {
        Console.Write("Введите код магазина: ");
        string storeCode = Console.ReadLine();

        List<Product> products = storeService.GetProductsByStore(storeCode);

        Console.WriteLine($"Товары в магазине с кодом {storeCode}:");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Name} - {product.Quantity} шт. - Цена: {product.Price}");
        }
    }

    private static void ImportProductsToStore()
    {
        Console.Write("Введите код магазина: ");
        string storeCode = Console.ReadLine();

        Console.Write("Введите название товара: ");
        string productName = Console.ReadLine();

        Console.Write("Введите количество товара: ");
        int quantity = int.Parse(Console.ReadLine());

        Console.Write("Введите цену товара: ");
        decimal price = decimal.Parse(Console.ReadLine());

        List<ProductQuantity> products = new List<ProductQuantity>
        {
            new ProductQuantity { ProductName = productName, Quantity = quantity, Price = price }
        };

        storeService.StockProducts(storeCode, products);

        Console.WriteLine("Товары успешно добавлены в магазин!");
    }

    private static void FindCheapestStoreForProduct()
    {
        Console.Write("Введите название товара: ");
        string productName = Console.ReadLine();
        string cheapestStore = storeService.FindCheapestStoreForProduct(productName);
        if (cheapestStore != null)
            {
                Console.WriteLine($"Самый дешевый магазин для товара '{productName}': {cheapestStore}");
            }
            else
            {
                Console.WriteLine($"Товар '{productName}' не найден в магазинах.");
            }
        }

    private static void GetAffordableProductsInStore()
    {
        Console.Write("Введите код магазина: ");
        string storeCode = Console.ReadLine();
        Console.Write("Введите бюджет: ");
        decimal budget = decimal.Parse(Console.ReadLine());
        List<Product> affordableProducts = storeService.GetAffordableProducts(storeCode, budget);
        if (affordableProducts.Count > 0)
            {   
                Console.WriteLine($"Товары, которые можно купить в магазине '{storeCode}' на бюджет {budget} рублей:");
                foreach (var product in affordableProducts)
                {
                    Console.WriteLine($"{product.Name} - {product.Quantity} шт. - Цена: {product.Price}");
                }
            }
            else
            {
                Console.WriteLine($"В магазине '{storeCode}' нет товаров, соответствующих бюджету.");
            }
    }

    private static void PurchaseProductsInStore()
    {
        Console.Write("Введите код магазина: ");
        string storeCode = Console.ReadLine();

        Console.Write("Введите количество разных товаров в партии: ");
        int numberOfProducts = int.Parse(Console.ReadLine());

        List<ProductQuantity> productsToBuy = new List<ProductQuantity>();
        for (int i = 0; i < numberOfProducts; i++)
        {
            Console.Write($"Введите название товара {i + 1}: ");
            string productName = Console.ReadLine();

            Console.Write($"Введите количество товара {i + 1}: ");
            int quantity = int.Parse(Console.ReadLine());

            productsToBuy.Add(new ProductQuantity { ProductName = productName, Quantity = quantity });
        }

        decimal totalCost = storeService.PurchaseProductsInStore(storeCode, productsToBuy);

        if (totalCost >= 0)
        {
            Console.WriteLine($"Покупка успешно совершена. Общая стоимость: {totalCost} рублей");
        }
        else
        {
            Console.WriteLine($"Покупка не удалась. Недостаточно товаров в магазине.");
        }
    }

    private static void FindCheapestStoreForProductSet()
    {
        Console.Write("Введите количество разных товаров в партии: ");
        int numberOfProducts = int.Parse(Console.ReadLine());

        List<ProductQuantity> productsToBuy = new List<ProductQuantity>();
        for (int i = 0; i < numberOfProducts; i++)
        {
            Console.Write($"Введите название товара {i + 1}: ");
            string productName = Console.ReadLine();

            Console.Write($"Введите количество товара {i + 1}: ");
            int quantity = int.Parse(Console.ReadLine());

            productsToBuy.Add(new ProductQuantity { ProductName = productName, Quantity = quantity });
        }

        string cheapestStore = storeService.FindCheapestStoreForProductSet(productsToBuy);

        if (cheapestStore != null)
        {
            Console.WriteLine($"Самый дешевый магазин для данной партии товаров: {cheapestStore}");
        }
        else
        {
            Console.WriteLine("Для данной партии товаров магазины не найдены.");
        }
    }

    private static IStoreRepository CreateStoreRepository(string implementationType)
    {
        if (implementationType == "file")
        {
            return new FileStoreRepository();
        }
        else if (implementationType == "sqlite")
        {
            var dbContext = new SQLiteDbContext();
            dbContext.Database.EnsureCreated();
            return new SQLiteStoreRepository(dbContext);
        }
        else
        {
            return new FileStoreRepository();
        }
    }

    private static IStoreService CreateStoreService(string implementationType, IStoreRepository storeRepo, IProductRepository productRepo)
    {
        Console.WriteLine($"Имплементация из .property: {implementationType}");
        if (implementationType == "file")
        {
            return new StoreService(storeRepo, productRepo);
        }
        else if (implementationType == "sqlite")
        {
            return new SQLiteStoreService(storeRepo, productRepo);
        }
        else
        {
            throw new ArgumentException("Недопустимый тип реализации", nameof(implementationType));
        }
    }
}
