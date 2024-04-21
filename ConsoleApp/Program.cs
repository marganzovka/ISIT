using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Запрос пользователя на ввод имен файлов или команды
        Console.WriteLine("Введите названия файлов с расширениями через пробел или 'обработать все файлы':");
        string input = Console.ReadLine();

        // Если пользователь выбрал "обработать все файлы", запускаем функцию обработки всех файлов
        if (input.ToLower() == "обработать все файлы")
            ProcessAllFiles();
        // В противном случае обрабатываем только выбранные файлы
        else
            ProcessSelectedFiles(input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

        Console.WriteLine("Готово!");
    }

    // Функция обработки выбранных файлов
    static void ProcessSelectedFiles(string[] fileNames)
    {
        // Путь к папке с файлами
        string folderPath = "../../../../res";

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

    // Функция обработки всех файлов в папке
    static void ProcessAllFiles()
    {
        string folderPath = "../../../res";

        // Обработка каждого файла в папке
        foreach (string filePath in Directory.GetFiles(folderPath))
        {
            // Анализ файла и запись результатов в файл
            var results = AnalyzeFile(filePath);
            WriteResultsToFile(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileName(filePath) + ".tab"), results);
        }
    }

    // Функция анализа файла для подсчета частоты байтов
    static Dictionary<byte, int> AnalyzeFile(string filePath)
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
    static void WriteResultsToFile(string outputPath, Dictionary<byte, int> byteFrequency)
    {
        int totalBytes = byteFrequency.Sum(pair => pair.Value);
        double entropy = CalculateEntropy(byteFrequency);

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine($"{totalBytes} Размер файла");
            writer.WriteLine($"{entropy} Энтропия\n");
            writer.WriteLine($"Байт\t\tЧастота");
            foreach (var entry in byteFrequency.OrderByDescending(pair => pair.Value))
            {
                writer.WriteLine($"{entry.Key}\t\t{entry.Value}");
            }
        }
    }

    // Функция для расчета энтропии
    static double CalculateEntropy(Dictionary<byte, int> byteFrequency)
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
