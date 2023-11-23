using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExamWpfApp.Models.Enums;
using Newtonsoft.Json;

namespace ExamWpfApp.Models.Dictionary;

public class LanguageDictionary
{
    [JsonProperty("Dictionary")]
    private readonly List<DictionaryPart> dictionary;

    public LanguageTypes FromLanguage { get; }
    public LanguageTypes ToLanguage { get; }

    public List<DictionaryPart> GetDictionary() => dictionary;

    [System.Text.Json.Serialization.JsonConstructor]
    public LanguageDictionary(List<DictionaryPart> dictionary, LanguageTypes fromLanguage, LanguageTypes toLanguage)
    {
        FromLanguage = fromLanguage;
        ToLanguage = toLanguage;
        this.dictionary = dictionary ?? new List<DictionaryPart>();
    }
    public LanguageDictionary(LanguageTypes fromLanguage, LanguageTypes toLanguage) : this(null, fromLanguage, toLanguage) { }
    public LanguageDictionary() : this(null, LanguageTypes.None, LanguageTypes.None) { }

    public void AddWord(DictionaryPart value)
    {
        if (value.FromLanguage != FromLanguage && value.ToLanguage != ToLanguage)
            throw new Exception($"Слово {value} не может быть добавлено в словарь из-за несоотвествия языков!");

        if (dictionary.Contains(value))
            dictionary.Find(dictPart => dictPart.Word == value.Word)
                      .AddTranslation(value.Translation);
        else
            dictionary.Add(value);
    }

    public void RemoveWord(string word)
    {
        if (FindWord(word, out DictionaryPart dictPart) && dictionary.Remove(dictPart))
            return;
        else
            throw new Exception($"Ошибка в удалении слова {word}!");
    }

    /// <summary>
    /// True if word found, otherwise false
    /// </summary>
    /// <param name="word">Искомое слово</param>
    /// <param name="result">Результат поиска</param>
    public bool FindWord(string word, out DictionaryPart result)
    {
        result = dictionary.Find(dictPart => dictPart.Word == word);
        return result != null;
    }
    public bool FindWordsByTranslation(string translation, out List<DictionaryPart> result)
    {
        result = dictionary.Where(dictPart => dictPart.Translation.Contains(translation)).ToList();
        return result != null;
    }
    public void ReplaceWord(string oldWord, string newWord)
    {
        if (FindWord(oldWord, out DictionaryPart dictPart))
            dictPart.Word = newWord;
    }
    public bool DeleteWord(string word)
    {
        if (FindWord(word, out DictionaryPart dictPart))
        {
            return dictionary.Remove(dictPart);
        }
        return false;
    }
    public void ReplaceTranslation(string oldTranslation, string newTranslation)
    {
        FindWordsByTranslation(oldTranslation, out var resultList);
        foreach (DictionaryPart dictPart in dictionary.Where(result => resultList.Contains(result)))
        {
            for (int i = 0; i < dictPart.Translation.Count; i++)
            {
                if (dictPart.Translation[i] == oldTranslation)
                    dictPart.Translation[i] = newTranslation;
            }
        }
    }
    public void RemoveTranslation(string translation)
    {
        if (FindWordsByTranslation(translation, out var wordsList))
        {
            foreach (var word in wordsList)
            {
                word.RemoveTranslation(translation);
            }
        }
        else
            throw new Exception($"Ошибка в удалении перевода {translation}!");
    }


    public void RemoveTranslation(string word, string translation)
    {
        if (FindWord(word, out DictionaryPart dictPart))
            dictPart.RemoveTranslation(translation);
        else
            throw new Exception($"Ошибка в удалении перевода {translation} в слове {word}!");
    }
    public void AddTranslation(string word, string translation)
    {
        if (FindWord(word, out DictionaryPart dictPart))
            dictPart.AddTranslation(translation);
        else
            throw new Exception($"Ошибка в добавлении перевода {translation} в слове {word}!");
    }
    public List<string> this[string word]
    {
        get
        {
            return dictionary.Where(dictPart => dictPart.Word == word)
                             .SelectMany(dictPart => dictPart.Translation)
                             .ToList();
        }
    }
    public override string ToString()
    {
        StringBuilder sb = new($"\n\t{FromLanguage}-{ToLanguage} словарь\n");
        foreach (var dictionaryPart in dictionary)
        {
            sb.AppendLine(dictionaryPart.ToString());
        }
        return sb.ToString();
    }
}
