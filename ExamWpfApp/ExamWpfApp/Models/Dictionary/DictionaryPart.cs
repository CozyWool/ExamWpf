using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ExamWpfApp.Models.Enums;

namespace ExamWpfApp.Models.Dictionary;

public class DictionaryPart
{
    public string Word { get; set; }
    public List<string> Translation { get; set; }
    public LanguageTypes FromLanguage { get; }
    public LanguageTypes ToLanguage { get; }

    public string ShowTranslations
    {
        get => string.Join(", ", Translation);
        set => Translation = value.Split(", ").ToList();
    }

    [JsonConstructor]
    public DictionaryPart(string word, List<string> translation, LanguageTypes fromLanguage, LanguageTypes toLanguage)
    {
        FromLanguage = fromLanguage;
        ToLanguage = toLanguage;
        Word = word;
        Translation = translation;
    }

    public DictionaryPart() : this("", new List<string>(), LanguageTypes.None, LanguageTypes.None)
    {
    }

    public DictionaryPart(LanguageTypes fromLanguage, LanguageTypes toLanguage) :
        this("", new List<string>(), fromLanguage, toLanguage)
    {
    }

    public void AddTranslation(string translation) => Translation.Add(translation);
    public void AddTranslation(List<string> translation) => Translation.AddRange(translation);

    public void RemoveTranslation(string translation)
    {
        if (Translation.Count > 1)
            Translation.Remove(translation);
    }

    public void RemoveTranslation(List<string> translation)
    {
        if (Translation.Count - translation.Count >= 1)
            foreach (var translationItem in Translation)
                Translation.Remove(translationItem);
    }

    public override string ToString()
    {
        StringBuilder sb = new($"{Word}: ");
        sb.Append(string.Join(", ", Translation));
        return sb.ToString();
    }
}