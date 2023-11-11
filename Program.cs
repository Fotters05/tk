using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class Figure
{
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Figure() { }

    public Figure(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileEditor
{
    private string filePath;

    public FileEditor(string filePath)
    {
        this.filePath = filePath;
    }

    public void OpenFile()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не существует.");
            return;
        }

        string extension = Path.GetExtension(filePath).ToLower();

        if (extension == ".txt")
        {
            OpenTextFile();
        }
        else if (extension == ".json")
        {
            OpenJsonFile();
        }
        else if (extension == ".xml")
        {
            OpenXmlFile();
        }
        else
        {
            Console.WriteLine("Неподдерживаемый формат файла.");
        }
    }

    public void SaveFile(Figure figure)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        if (extension == ".txt")
        {
            SaveTextFile(figure);
        }
        else if (extension == ".json")
        {
            SaveJsonFile(figure);
        }
        else if (extension == ".xml")
        {
            SaveXmlFile(figure);
        }
        else
        {
            Console.WriteLine("Неверный формат файла.");
        }
    }

    private void OpenTextFile()
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }

    private void OpenJsonFile()
    {
        string json = File.ReadAllText(filePath);
        Figure figure = JsonConvert.DeserializeObject<Figure>(json);
        Console.WriteLine($"Name: {figure.Name}, Width: {figure.Width}, Height: {figure.Height}");
    }

    private void OpenXmlFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure));
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            Figure figure = (Figure)serializer.Deserialize(stream);
            Console.WriteLine($"Name: {figure.Name}, Width: {figure.Width}, Height: {figure.Height}");
        }
    }

    private void SaveTextFile(Figure figure)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine($"Name: {figure.Name}");
            writer.WriteLine($"Width: {figure.Width}");
            writer.WriteLine($"Height: {figure.Height}");
        }
    }

    private void SaveJsonFile(Figure figure)
    {
        string json = JsonConvert.SerializeObject(figure);
        File.WriteAllText(filePath, json);
    }

    private void SaveXmlFile(Figure figure)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure));
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, figure);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь файла: ");
        string filePath = Console.ReadLine();
        FileEditor editor = new FileEditor(filePath);

        ConsoleKeyInfo keyInfo;
        do
        {
            Console.WriteLine("Для открытия файла нажмите Enter, для сохранения файла нажмите F1, для выхода - Escape.");
            keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                editor.OpenFile();
            }
            else if (keyInfo.Key == ConsoleKey.F1)
            {
                Console.WriteLine("Введите через запятую название, длину, ширину: ");
                string[] input = Console.ReadLine().Split(',');
                if (input.Length == 3)
                {
                    string name = input[0].Trim();
                    double width, height;
                    if (double.TryParse(input[1].Trim(), out width) && double.TryParse(input[2].Trim(), out height))
                    {
                        Figure figure = new Figure(name, width, height);
                        editor.SaveFile(figure);
                        Console.WriteLine("Файл успешно сохранен.");
                    }
                    else
                    {
                        Console.WriteLine("Неверные данные для фигуры.");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный формат ввода.");
                }
            }
        } while (keyInfo.Key != ConsoleKey.Escape);
    }
}