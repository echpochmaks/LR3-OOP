using System;
using System.IO;

public class ConfigurationReader
{
    public static string ReadImplementationTypeFromConfig()
    {
        string configFilePath = ".property";

        if (File.Exists(configFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(configFilePath);
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("implementationType="))
                    {
                        return line.Split('=')[1].Trim().ToLower();
                    }
                }

                Console.WriteLine("Не удалось найти значение implementationType в файле конфигурации.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла конфигурации: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Файл конфигурации не найден.");
        }

        return "file";
    }
}