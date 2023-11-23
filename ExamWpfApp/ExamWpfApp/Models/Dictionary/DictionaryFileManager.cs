using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExamWpfApp.Models.Dictionary;

public class DictionaryFileManager
{
    public string DirectoryPath { get; private set; }
    public List<FileInfo> Files { get; private set; }
    public DictionaryFileManager(string directoryPath)
    {
        DirectoryPath = directoryPath;
        UpdateFiles(DirectoryPath);
    }

    private void UpdateFiles(string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);
        if (!Directory.Exists(DirectoryPath)) return;
        
        DirectoryInfo info = new(DirectoryPath);
        Files = info.GetFiles().Where(file => file.Name.EndsWith(".json")).ToList();
    }

    public DictionaryFileManager() : this(@"..\..\..\Dictionaries") { }

    public bool SaveDictionary(LanguageDictionary? langDict, out string savePath)
    {
        savePath = $@"{DirectoryPath}\{langDict.FromLanguage}-{langDict.ToLanguage}-Dictionary.json";
        FileInfo fileInfo = new(savePath);
        if (fileInfo.Exists)
        {
            Console.Write("Файл уже существует, перезаписать?(Y/N): "); // неудобно было бы делать проверку вне метода
            if (Console.ReadLine().ToUpper() != "Y") return false;
        }

        var json = JsonConvert.SerializeObject(langDict, Formatting.Indented, new StringEnumConverter());
        using var sw = new StreamWriter(savePath);
        sw.WriteLine(json);
        return true;
    }
    public LanguageDictionary? Load(int IndexOfFile)
    {
        if (IndexOfFile < 0 && IndexOfFile >= Files.Count) return null; // TODO: добавить exception
        using var sr = new StreamReader(Path.Combine(DirectoryPath, Files[IndexOfFile].Name));
        var json = sr.ReadToEnd();
        var deserialized = JsonConvert.DeserializeObject<LanguageDictionary>(json, new StringEnumConverter());
        return deserialized;
    }
    public bool Delete(int IndexOfFile)
    {
        if (IndexOfFile < 0 && IndexOfFile >= Files.Count) return false; // TODO: добавить exception
        var filePath = Path.Combine(DirectoryPath, Files[IndexOfFile].Name);
        if (File.Exists(filePath))
            File.Delete(filePath);
        if (!File.Exists(filePath)) // Проверка после удаления
            return true;
        return false;
    }

    public void ShowDictionaries()
    {
        UpdateFiles(DirectoryPath);
        if (!Directory.Exists(DirectoryPath)) return;
        for (var i = 0; i < Files.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {Files[i]}");
        }
    }
    public void ShowDictionaries(string Path)
    {
        if (!Directory.Exists(Path)) return;
        var info = new DirectoryInfo(Path);
        var fileInfos = info.GetFiles();
        for (var i = 0; i < fileInfos.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {fileInfos[i]}");
        }
    }
    public static void ExportWordToFile(DictionaryPart dictPart, string path)
    {
        using var sw = new StreamWriter(path);
        var json = JsonConvert.SerializeObject(dictPart, Formatting.Indented, new StringEnumConverter());
        sw.WriteLine(json);
    }
}
