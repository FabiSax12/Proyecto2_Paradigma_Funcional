using Avalonia.Controls;
using Proyecto2_Lenguajes.GUI.ViewModels;

namespace Proyecto2_Lenguajes.GUI.Views;

public partial class AhorcadoWindow : Window
{
    public AhorcadoWindow()
    {
        InitializeComponent();
        DataContext = new AhorcadoViewModel();
    }
}
