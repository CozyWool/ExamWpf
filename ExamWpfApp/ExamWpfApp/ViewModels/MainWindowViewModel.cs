using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ExamWpfApp.Models.Commands;
using ExamWpfApp.Models.Dictionary;
using ExamWpfApp.Models.Enums;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace ExamWpfApp.ViewModels;

// TODO: Убрать зависимость от регистра для удобства пользования
public class MainWindowViewModel : INotifyPropertyChanged
{
    private LanguageDictionary _currentDictionary;

    public MainWindowViewModel(LanguageDictionary dictionary, Window owner)
    {
        _currentDictionary = dictionary;
        Owner = owner;
        RemoveWordCommand = new DelegateCommand(obj =>
        {
            if (obj is not DictionaryPart dictionaryPart)
            {
                return;
            }

            Delete(dictionaryPart.Word);
        });
        ExportWordCommand = new DelegateCommand(obj =>
        {
            if (obj is not DictionaryPart dictionaryPart)
            {
                return;
            }

            ExportWord(dictionaryPart);
        });
        CreateDictionaryCommand = new DelegateCommand(_ => CreateDictionary());
        ExitCommand = new DelegateCommand(_ => Exit());
        AddWordComamnd = new DelegateCommand(_ => AddWord());
        LoadDictionaryCommand = new DelegateCommand(_ => Load());
        SaveDictionaryCommand = new DelegateCommand(_ => Save());
        FindCommand = new DelegateCommand(obj => Find(obj as string));

        DictionaryFileManager = new DictionaryFileManager();
        owner.Closing += OwnerOnClosing;
    }

    public LanguageDictionary CurrentDictionary
    {
        get => _currentDictionary;
        set
        {
            if (Equals(value, _currentDictionary))
                return;
            _currentDictionary = value;
            OnPropertyChanged();
        }
    }

    public Window Owner { get; set; }
    public DictionaryFileManager DictionaryFileManager { get; }
    public Command RemoveWordCommand { get; }
    public Command ExportWordCommand { get; }
    public Command CreateDictionaryCommand { get; }
    public Command ExitCommand { get; }
    public Command AddWordComamnd { get; }
    public Command LoadDictionaryCommand { get; }
    public Command SaveDictionaryCommand { get; }
    public Command FindCommand { get; }
    
    private void OwnerOnClosing(object? sender, CancelEventArgs e) => ExitWithoutClosing();

    private void Load()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "JSON файлы(*.json)|*.json",
            RestoreDirectory = true
        };
        {
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                CurrentDictionary =
                    DictionaryFileManager.Load(dialog.FileName) ?? throw new InvalidOperationException();
            }
        }
    }

    private void CreateDictionary()
    {
        var continueResult = MessageBox.Show("Несохраненные данные будут утеряны!\nПродолжить?", "Внимание!",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (continueResult == MessageBoxResult.No)
            return;

        var createWindow = new CreateDictionaryWindow(Owner);
        var windowViewModel = new CreateDictionaryViewModel(createWindow);
        createWindow.DataContext = windowViewModel;

        if (createWindow.ShowDialog() != true)
            return;

        if (createWindow.DataContext is not CreateDictionaryViewModel viewModel)
            return;

        CurrentDictionary = viewModel.Dictionary;

        var saveResult = MessageBox.Show($"\tСловарь создан. Сохранить?: ", "Сохранить?",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (MessageBoxResult.Yes == saveResult)
            Save();
    }

    private void AddWord()
    {
        CurrentDictionary.AddWord(new DictionaryPart(CurrentDictionary.FromLanguage, CurrentDictionary.ToLanguage));
    }

    private void Delete(string wordToRemove)
    {
        CurrentDictionary.DeleteWord(wordToRemove);
    }

    private void Delete(DictionaryPart dictionaryPartToRemove)
    {
        CurrentDictionary.DeleteWord(dictionaryPartToRemove);
    }

    private void Delete(string word, string translationToRemove)
    {
        CurrentDictionary.RemoveTranslation(word, translationToRemove);
    }

    private static void ExportWord(DictionaryPart word)
    {
        var exportFilePath = "";
        using var dialog = new SaveFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt",
            RestoreDirectory = true
        };
        {
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                exportFilePath = dialog.FileName;
            }
        }

        using StreamWriter sw = new(exportFilePath, true);
        sw.WriteLine(word.ToString());
        MessageBox.Show("Слово записано.");
    }

    private void Save()
    {
        string savePath;

        using var dialog = new SaveFileDialog
        {
            Filter = "JSON файлы(*.json)|*.json",
            RestoreDirectory = true
        };
        {
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                savePath = dialog.FileName;
            }
            else
            {
                return;
            }
        }
        MessageBox.Show(DictionaryFileManager.SaveDictionary(CurrentDictionary, savePath)
            ? $"Файл сохранен в папку программы под названием: {savePath}."
            : "Произошла ошибка при сохранении файла.", "Сохранение");
    }

    private void Find(string? s)
    {
        if (s == null)
            return;
        s = s.Trim();

        var sb = new StringBuilder();
        if (CurrentDictionary.FindWord(s, out var word))
        {
            sb.AppendLine(word.ToString());
        }

        if (CurrentDictionary.FindWordsByTranslation(s, out var result))
        {
            sb.Append(string.Join("\n", result));
        }

        MessageBox.Show(sb.Length > 0
            ? $"Слово(-а) найдено(-ы):\n{sb}"
            : "Слово не найдено.");
    }

    private void ExitWithoutClosing()
    {
        var saveResult = MessageBox.Show($"Сохранить словарь перед выходом?: ", "Сохранить?",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (MessageBoxResult.Yes == saveResult)
            Save();
    }

    private void Exit()
    {
        ExitWithoutClosing();
        Application.Current.Shutdown();
        Owner.Close();
    }


    public void PrintDict()
    {
        MessageBox.Show($"\tТекущий словарь: {CurrentDictionary}\n");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}