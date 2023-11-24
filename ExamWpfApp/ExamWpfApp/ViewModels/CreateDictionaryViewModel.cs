using System.Windows;
using ExamWpfApp.Models.Commands;
using ExamWpfApp.Models.Dictionary;
using ExamWpfApp.Models.Enums;

namespace ExamWpfApp.ViewModels;

public class CreateDictionaryViewModel
{
    private bool CanOkClick() => Dictionary.FromLanguage != LanguageTypes.None &&
                                 Dictionary.ToLanguage != LanguageTypes.None &&
                                 Dictionary.FromLanguage != Dictionary.ToLanguage;

    public CreateDictionaryViewModel(Window owner)
    {
        Owner = owner;
        Dictionary = new LanguageDictionary();
        OkCommand = new DelegateCommand(OkClick, _ => CanOkClick());
        CancelCommand = new DelegateCommand(CancelClick);
    }

    public Window Owner { get; }

    public LanguageDictionary Dictionary { get; }
    
    public Command OkCommand { get; }
    public Command CancelCommand { get; }

    private void OkClick(object obj)
    {
        Owner.DialogResult = true;
        Owner.Close();
    }

    private void CancelClick(object obj)
    {
        Owner.DialogResult = false;
        Owner.Close();
    }
}