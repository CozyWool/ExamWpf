using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExamWpfApp.Models.Dictionary;
using ExamWpfApp.Models.Enums;


namespace ExamWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CurrentDictionary = null;
            DictionaryFileManager = new DictionaryFileManager();
            ExportFileName = "Words";
            ExportFileDirectoryName = @$"..\..\..\ExportedWords";
            ExportFileDirectoryPath = $"{ExportFileDirectoryName}/{ExportFileName}.txt";
            Directory.CreateDirectory(ExportFileDirectoryName);
            InitializeComponent();
        }

        public LanguageDictionary? CurrentDictionary { get; set; }
        public DictionaryFileManager DictionaryFileManager { get; set; }
        public string ExportFileName { get; set; }
        public string ExportFileDirectoryName { get; set; }
        public string ExportFileDirectoryPath { get; set; }
        
// TODO: Добавить везде защиту от дурака
// TODO: Добавить оповещение успех/не успех работы функции
// TODO: Убрать зависимость от регистра для удобства пользования

        void Do()
        {
            while (true)
            {
                Console.WriteLine("\n\tПрограмма \"Словарь\"\n");
                Console.WriteLine("1 - Создать словарь");
                Console.WriteLine("2 - Добавить слово/перевод в словарь");
                Console.WriteLine("3 - Удалить слово/перевод в словаре");
                Console.WriteLine("4 - Заменить слово/перевод в словаре");
                Console.WriteLine("5 - Найти слово/перевод в словаре");
                Console.WriteLine("6 - Сохранить словарь в файл");
                Console.WriteLine("7 - Открыть словарь из файла");
                Console.WriteLine("8 - Удалить словарь и его файл");
                Console.WriteLine("9 - Экспортировать слово/перевод в отдельный файл");
                Console.WriteLine("10 - Посмотреть текущий словарь");
                Console.WriteLine("0 - Выйти");
                Console.Write("\nВведите ответ: ");
                var t = MenuTypesHelper.Parse(Console.ReadLine());

                PrintDict(CurrentDictionary);

                switch (t)
                {
                    case MenuTypes.CreateDictionary:
                        CreateDictionary();
                        break;
                    case MenuTypes.Add:
                        if (NullTest(CurrentDictionary)) break;
                        Add();
                        break;
                    case MenuTypes.Delete:
                        if (NullTest(CurrentDictionary)) break;
                        Delete();
                        break;
                    case MenuTypes.Replace:
                        if (NullTest(CurrentDictionary)) break;
                        Replace();
                        break;
                    case MenuTypes.Find:
                        if (NullTest(CurrentDictionary)) break;
                        Find();
                        break;
                    case MenuTypes.Save:
                        if (NullTest(CurrentDictionary)) break;
                        Save();
                        break;
                    case MenuTypes.Load:
                        Load();
                        break;
                    case MenuTypes.DeleteDictionary:
                        DeleteDictionary();
                        break;
                    case MenuTypes.ExportWord:
                        if (NullTest(CurrentDictionary)) break;
                        ExportWord();
                        break;
                    case MenuTypes.Exit:
                        StopProgram();
                        return;
                    case MenuTypes.Show:
                        // оно и так показывается
                        break;
                    default:
                        break;
                }
            }
        }

        void Load()
        {
            Console.Clear();
            Console.WriteLine("\tСписок словарей");
            DictionaryFileManager.ShowDictionaries();
            int fileCount = DictionaryFileManager.Files.Count;

            int answer = 0;
            while (answer < 1 || answer > fileCount)
            {
                try
                {
                    Console.WriteLine("Ваш выбор(0 - для возврата в меню): ");
                    answer = int.Parse(Console.ReadLine());
                    if (answer == 0)
                    {
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            CurrentDictionary = DictionaryFileManager.Load(answer - 1);
            PrintDict(CurrentDictionary);
        }

        void CreateDictionary()
        {
            // TODO: сделать предупреждение о потере несохранненых данных
            string[] typesArray = Enum.GetNames<LanguageTypes>();
            for (int i = 1; i < typesArray.Length; i++)
            {
                string type = typesArray[i];
                Console.WriteLine($"{i} - {type}");
            }

            Console.Write("Введите с какого языка вы хотите переводить: ");
            LanguageTypes fromLanguage = Enum.Parse<LanguageTypes>(Console.ReadLine());
            Console.Write("Введите на какой языка вы хотите переводить: ");
            LanguageTypes toLanguage = Enum.Parse<LanguageTypes>(Console.ReadLine());
            CurrentDictionary = new LanguageDictionary(fromLanguage, toLanguage);
            Console.WriteLine($"\tСловарь создан {CurrentDictionary}");
            Console.Write("Сохранить?(Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y") Save();
        }

        void Add()
        {
            Console.WriteLine("Что вы хотите удалить?");
            Console.WriteLine("1 - Добавить слово");
            Console.WriteLine("2 - Добавить перевод");
            Console.WriteLine("0 - Вернуться в меню");
            int answer = -1;
            while (answer < 0 || answer > 2)
            {
                try
                {
                    Console.Write("Ваш выбор: ");
                    answer = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            string word;
            switch (answer)
            {
                case 1:
                    Console.Write($"Введите слово на {CurrentDictionary.FromLanguage}: ");
                    word = Console.ReadLine().Trim();
                    Console.Write($"Введите перевод(-ы) на {CurrentDictionary.ToLanguage} через ПРОБЕЛ: ");
                    List<string> translation = Console.ReadLine().Split().ToList();
                    CurrentDictionary.AddWord(new DictionaryPart(word, translation, CurrentDictionary.FromLanguage,
                        CurrentDictionary.ToLanguage));
                    break;
                case 2:
                    try
                    {
                        Console.Write($"Введите слово, которому нужно добавить перевод: ");
                        word = Console.ReadLine().Trim();
                        Console.Write($"Введите перевод слова: ");
                        string translationWord = Console.ReadLine().Trim();
                        CurrentDictionary.AddTranslation(word, translationWord);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    break;
                case 0:
                    Console.WriteLine("Возращащение в меню...");
                    return;
                default:
                    return;
            }

            Console.Clear();
            Console.WriteLine($"\tТекущий словарь: {CurrentDictionary}\n");
        }

        void Delete()
        {
            Console.WriteLine("Что вы хотите удалить?");
            Console.WriteLine("1 - Удалить слово");
            Console.WriteLine("2 - Удалить перевод");
            Console.WriteLine("0 - Вернуться в меню");
            int answer = -1;
            while (answer < 0 || answer > 2)
            {
                try
                {
                    Console.Write("Ваш выбор: ");
                    answer = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            string word;
            switch (answer)
            {
                case 1:
                    Console.Write("Введите слово, которое нужно удалить: ");
                    word = Console.ReadLine().Trim();
                    CurrentDictionary.DeleteWord(word);
                    break;
                case 2:
                    try
                    {
                        Console.Write($"Введите слово, которому нужно удалить перевод: ");
                        word = Console.ReadLine().Trim();
                        Console.Write($"Введите перевод слова, которое нужно удалить: ");
                        string translation = Console.ReadLine().Trim();
                        CurrentDictionary.RemoveTranslation(word, translation);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    break;
                case 0:
                    Console.WriteLine("Возращащение в меню...");
                    return;
                default:
                    return;
            }

            Console.Clear();
            Console.WriteLine($"\tТекущий словарь: {CurrentDictionary}\n");
        }

        void Replace()
        {
            Console.WriteLine("Что вы хотите заменить?");
            Console.WriteLine("1 - Заменить слово");
            Console.WriteLine("2 - Заменить перевод");
            Console.WriteLine("0 - Вернуться в меню");
            int answer = -1;
            while (answer < 0 || answer > 2)
            {
                try
                {
                    Console.WriteLine("Ваш выбор: ");
                    answer = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            switch (answer)
            {
                case 1:
                    Console.Write("Введите старое слово: ");
                    string oldWord = Console.ReadLine().Trim();
                    Console.Write("Введите новое слово: ");
                    string newWord = Console.ReadLine().Trim();
                    CurrentDictionary.ReplaceWord(oldWord, newWord);
                    break;
                case 2:
                    Console.Write($"Введите старый перевод слова: ");
                    string oldTranslation = Console.ReadLine().Trim();
                    Console.Write($"Введите новый перевод слова: ");
                    string newTranslation = Console.ReadLine().Trim();
                    CurrentDictionary.ReplaceTranslation(oldTranslation, newTranslation);
                    break;
                case 0:
                    Console.WriteLine("Возращащение в меню...");
                    return;
                default:
                    return;
            }

            Console.Clear();
            Console.WriteLine($"\tТекущий словарь: {CurrentDictionary}\n");
        }

        void ExportWord()
        {
            Console.Write("Введите слово, которое хотите сохранить:");
            string word = Console.ReadLine().Trim();
            if (!CurrentDictionary.FindWord(word, out DictionaryPart result))
            {
                Console.WriteLine("Слово не найдено, возврат в главное меню.");
                return;
            }

            // Не уверен добавлять ли эту возможность
            //Console.Write($"Введите название папки, в которую нужно сохранить слово(0, если сохранить в {ExportFileDirectoryName}):");
            //var ans = Console.ReadLine().Trim();
            //if(ans != "0") 
            //    ExportFileDirectoryName = @$"..\..\..\{ans}";

            Console.Write(
                $"Введите название файла, в который нужный сохранить слово(0, если сохранить в {ExportFileName}):");
            var ans = Console.ReadLine().Trim();
            if (ans != "0")
                ExportFileName = ans;
            ExportFileDirectoryPath = $"{ExportFileDirectoryName}/{ExportFileName}.txt";
            Directory.CreateDirectory(ExportFileDirectoryName);
            if (!Directory.Exists(ExportFileDirectoryName))
            {
                Console.WriteLine("Не удалось создать папку для сохранения файла...");
            }

            using StreamWriter sw = new(ExportFileDirectoryPath, true); // Надо было в XML или в TXT?
            sw.WriteLine(result.ToString());
            Console.WriteLine("Слово записано.");
        }


        void DeleteDictionary()
        {
            Console.Clear();
            Console.WriteLine("\tСписок словарей");
            DictionaryFileManager.ShowDictionaries();
            int fileCount = DictionaryFileManager.Files.Count;

            int answer = 0;
            while (answer < 1 || answer > fileCount)
            {
                try
                {
                    Console.WriteLine("Ваш выбор: ");
                    answer = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            if (!DictionaryFileManager.Delete(answer - 1))
                Console.WriteLine("Произошла ошибка при удалении, возврат в главное меню.");
            PrintDict(CurrentDictionary);
        }

        void Save()
        {
            if (DictionaryFileManager.SaveDictionary(CurrentDictionary, out var savePath))
                Console.WriteLine($"Файл сохранен в папку программы под названием: {savePath}.");
            else
                Console.WriteLine($"Произошла ошибка при сохранении файла.");
        }

        void Find()
        {
            Console.WriteLine("Что вы хотите удалить?");
            Console.WriteLine("1 - Найти слово");
            Console.WriteLine("2 - Найти перевод(-ы)");
            Console.WriteLine("0 - Вернуться в меню");
            int answer = -1;
            while (answer < 0 || answer > 2)
            {
                try
                {
                    Console.Write("Ваш выбор: ");
                    answer = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Произошла ошибка при вводе, попробуйте еще раз.");
                }
            }

            switch (answer)
            {
                case 1:
                    Console.Write("Введите перевод для поиска слов(-а): ");
                    string translation = Console.ReadLine().Trim();
                    CurrentDictionary.FindWordsByTranslation(translation, out var result);
                    Console.WriteLine();
                    foreach (var dictPart in result)
                    {
                        Console.WriteLine($"Слово найдено: {dictPart}");
                    }

                    break;
                case 2:
                    Console.Write("Введите слово для поиска его перевода(-ов): ");
                    string word = Console.ReadLine().Trim();
                    Console.WriteLine();
                    if (CurrentDictionary.FindWord(word, out var wordFound))
                        Console.WriteLine($"Слово найдено: {wordFound}");
                    else Console.WriteLine("Слово не найдено.");
                    break;
                case 0:
                    Console.WriteLine("Возращащение в меню...");
                    return;
                default:
                    return;
            }
        }

        void StopProgram()
        {
            Console.Clear();
            if (CurrentDictionary != null)
            {
                PrintDict(CurrentDictionary);
                Console.Write("Сохранить перед выходом?(Y/N): ");
                if (Console.ReadLine().ToUpper() == "Y") Save();
            }

            Console.WriteLine("\n\t\tДо свидания!\n");
            Console.ReadKey();
        }

        void PrintDict(LanguageDictionary? CurrentDictionary)
        {
            Console.Clear();
            Console.WriteLine(
                $"\tТекущий словарь: {(CurrentDictionary == null ? "Не выбран" : CurrentDictionary.ToString())}\n");
        }

        bool NullTest(LanguageDictionary? CurrentDictionary)
        {
            if (CurrentDictionary == null)
            {
                Console.WriteLine("Словарь не выбран! Для работы со словарем необходимо выбрать его.");
                return true;
            }

            return false;
        }
    }
}