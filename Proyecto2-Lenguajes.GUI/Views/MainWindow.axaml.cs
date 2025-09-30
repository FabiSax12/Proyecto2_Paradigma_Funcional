using Avalonia.Controls;
using Proyecto2_Lenguajes.GUI.ViewModels;

namespace Proyecto2_Lenguajes.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}