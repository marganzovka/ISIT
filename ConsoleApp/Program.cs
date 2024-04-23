using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

class Task
{
    // Путь к папке с файлами
    string folderPath = "../../../../ConsoleApp";
    public void Run()
    {
        // Запрос пользователя на ввод имен файлов или команды
        Console.WriteLine("Введите названия файлов с расширениями. Если их несколько, то вводите через пробел");
        string input = Console.ReadLine();
      
        ProcessSelectedFiles(input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

        Console.WriteLine("Готово!");
    }

    // Функция обработки выбранных файлов
    void ProcessSelectedFiles(string[] fileNames)
    {
        // Обработка каждого выбранного файла
        foreach (string fileName in fileNames)
        {
            string filePath = Path.Combine(folderPath, fileName);

            // Проверка существования файла
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл '{filePath}' не найден.");
                continue;
            }

            // Анализ файла и запись результатов в файл
            var results = AnalyzeFile(filePath);
            WriteResultsToFile(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath) + ".tab"), results);
        }
    }


    // Функция анализа файла для подсчета частоты байтов
    Dictionary<byte, int> AnalyzeFile(string filePath)
    {
        var byteFrequency = new Dictionary<byte, int>();
        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            int byteRead;
            while ((byteRead = fs.ReadByte()) != -1)
            {
                if (byteFrequency.ContainsKey((byte)byteRead))
                    byteFrequency[(byte)byteRead]++;
                else
                    byteFrequency[(byte)byteRead] = 1;
            }
        }
        return byteFrequency;
    }

    // Функция записи результатов в файл
    void WriteResultsToFile(string outputPath, Dictionary<byte, int> byteFrequency)
    {
        

        // Создание пути для результирующего файла в подпапке
        string resultFilePath = Path.Combine(folderPath, Path.GetFileName(outputPath));

        // Настройка форматирования чисел с использованием точки в качестве разделителя десятичных разрядов
        CultureInfo culture = CultureInfo.InvariantCulture;


        int totalBytes = byteFrequency.Sum(pair => pair.Value);
        double entropy = CalculateEntropy(byteFrequency);

        // Запись результатов в файл в подпапке
        using (StreamWriter writer = new StreamWriter(resultFilePath))
        {
            writer.WriteLine($"{totalBytes}\tРазмер файла");
            writer.WriteLine($"{entropy.ToString(culture)}\tЭнтропия");
            writer.WriteLine($"Байт\tЧастота");
            foreach (var entry in byteFrequency.OrderByDescending(pair => pair.Value))
            {
                writer.WriteLine($"{entry.Key}\t{entry.Value}");
            }
        }
    }

    // Функция для расчета энтропии
    double CalculateEntropy(Dictionary<byte, int> byteFrequency)
    {
        int totalBytes = byteFrequency.Sum(pair => pair.Value);
        double entropy = 0.0;
        foreach (int count in byteFrequency.Values)
        {
            double probability = (double)count / totalBytes;
            entropy -= probability * Math.Log(probability, 2);
        }
        return entropy;
    }
}



class Program
{
    static void Main(string[] args)
    {
        Task task = new Task();
        task.Run();

    }
}