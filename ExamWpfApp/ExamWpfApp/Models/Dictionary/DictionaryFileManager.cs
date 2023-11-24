using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MessageBox = System.Windows.MessageBox;

namespace ExamWpfApp.Models.Dictionary;

public class DictionaryFileManager
{
    public static bool SaveDictionary(LanguageDictionary langDict, string savePath)
    {
        try
        {
            FileInfo fileInfo = new(savePath);

            var json = JsonConvert.SerializeObject(langDict, Formatting.Indented, new StringEnumConverter());
            using var sw = new StreamWriter(savePath);
            sw.WriteLine(json);
        }
        catch (Exception)
        {
            return false;
        }
        
        return true;
    }
    public LanguageDictionary? Load(string dictionaryPath)
    {
        using var sr = new StreamReader(dictionaryPath);
        var json = sr.ReadToEnd();
        var deserialized = JsonConvert.DeserializeObject<LanguageDictionary>(json, new StringEnumConverter());
        return deserialized;
    }
    public static bool Delete(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        
        return !File.Exists(filePath);
    }
    public static void ExportWordToFile(DictionaryPart dictPart, string path)
    {
        using var sw = new StreamWriter(path);
        var json = JsonConvert.SerializeObject(dictPart, Formatting.Indented, new StringEnumConverter());
        sw.WriteLine(json);
    }
}
