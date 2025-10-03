using Avalonia.Controls;
using Avalonia.Interactivity;
using Proyecto2_Lenguajes.GUI.ViewModels;

namespace Proyecto2_Lenguajes.GUI.Views;

public partial class SopaLetrasWindow : Window
{
    public SopaLetrasWindow()
    {
        InitializeComponent();
        DataContext = new SopaLetrasViewModel(this);
    }

    private void CeldaButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is CeldaViewModel celda)
        {
            var viewModel = DataContext as SopaLetrasViewModel;
            viewModel?.ManejadorCeldaClick(celda);
        }
    }
}