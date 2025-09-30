using Avalonia.Controls;
using Proyecto2_Lenguajes.GUI.ViewModels;

namespace Proyecto2_Lenguajes.GUI.Views;

public partial class SopaLetrasWindow : Window
{
    public SopaLetrasWindow()
    {
        InitializeComponent();
        DataContext = new SopaLetrasViewModel();
    }
}