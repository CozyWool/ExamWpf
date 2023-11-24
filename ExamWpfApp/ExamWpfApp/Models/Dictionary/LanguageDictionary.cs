using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ExamWpfApp.Models.Enums;
using Newtonsoft.Json;

namespace ExamWpfApp.Models.Dictionary;

public class LanguageDictionary : INotifyPropertyChanged
{
    [JsonProperty("Dictionary")]
    private readonly ObservableCollection<DictionaryPart> _dictionary;

    private LanguageTypes _fromLanguage;
    private LanguageTypes _toLanguage;

    public LanguageTypes FromLanguage
    {
        get => _fromLanguage;
        set
        {
            if (Equals(value, _fromLanguage))
                return;
            _fromLanguage = value;
            OnPropertyChanged();
        }
    }

    public LanguageTypes ToLanguage
    {
        get => _toLanguage;
        set
        {
            if (Equals(value, _toLanguage))
                return;
            _toLanguage = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<DictionaryPart> DictionaryParts => _dictionary;


    [JsonConstructor]
    public LanguageDictionary(ObservableCollection<DictionaryPart> dictionary, LanguageTypes fromLanguage, LanguageTypes toLanguage)
    {
        FromLanguage = fromLanguage;
        ToLanguage = toLanguage;
        _dictionary = dictionary;
    }
    public LanguageDictionary(LanguageTypes fromLanguage, LanguageTypes toLanguage) : this(new ObservableCollection<DictionaryPart>(), fromLanguage, toLanguage) { }
    public LanguageDictionary() : this(new ObservableCollection<DictionaryPart>(), LanguageTypes.None, LanguageTypes.None) { }

    public void AddWord(DictionaryPart value)
    {
        if (value.FromLanguage != FromLanguage && value.ToLanguage != ToLanguage)
            throw new Exception($"Слово {value} не может быть добавлено в словарь из-за несоотвествия языков!");

        if (DictionaryParts.Contains(value))
        {
            DictionaryParts.FirstOrDefault(dictPart => dictPart.Word == value.Word)
                ?.AddTranslation(value.Translation);
        }
        else
        {
            DictionaryParts.Add(value);
        }
    }

    public void RemoveWord(string word)
    {
        if (FindWord(word, out var dictPart) && DictionaryParts.Remove(dictPart))
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
        result = DictionaryParts.FirstOrDefault(dictPart => dictPart.Word == word);
        return result != null;
    }
    public bool FindWordsByTranslation(string translation, out List<DictionaryPart> result)
    {
        result = DictionaryParts.Where(dictPart => dictPart.Translation.Contains(translation)).ToList();
        return result.Count > 0;
    }
    public void ReplaceWord(string oldWord, string newWord)
    {
        if (FindWord(oldWord, out DictionaryPart dictPart))
            dictPart.Word = newWord;
    }
    public bool DeleteWord(string word)
    {
        return FindWord(word, out DictionaryPart dictPart) && DictionaryParts.Remove(dictPart);
    }
    public bool DeleteWord(DictionaryPart dictionaryPartToDelete)
    {
        return DictionaryParts.Remove(dictionaryPartToDelete);
    }
    public void ReplaceTranslation(string oldTranslation, string newTranslation)
    {
        FindWordsByTranslation(oldTranslation, out var resultList);
        foreach (DictionaryPart dictPart in DictionaryParts.Where(result => resultList.Contains(result)))
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
            return DictionaryParts.Where(dictPart => dictPart.Word == word)
                             .SelectMany(dictPart => dictPart.Translation)
                             .ToList();
        }
    }
    public override string ToString()
    {
        StringBuilder sb = new($"\n\t{FromLanguage}-{ToLanguage} словарь\n");
        foreach (var dictionaryPart in DictionaryParts)
        {
            sb.AppendLine(dictionaryPart.ToString());
        }
        return sb.ToString();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
