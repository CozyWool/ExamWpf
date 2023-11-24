using System.Windows;

namespace ExamWpfApp;

public partial class CreateDictionaryWindow : Window
{
    public CreateDictionaryWindow(Window owner)
    {
        Owner = owner;
        InitializeComponent();
    }
}