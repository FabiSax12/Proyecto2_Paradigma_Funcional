using Avalonia.Media;
using Proyecto2_Lenguajes.GUI.ViewModels;
using ReactiveUI;

namespace Proyecto2_Lenguajes.GUI.ViewModels
{
  public class CeldaViewModel : ViewModelBase
  {
    private char _letra;
    private IBrush _color = Avalonia.Media.Brushes.White;
    private bool _estaSeleccionada;

    public char Letra
    {
      get => _letra;
      set => this.RaiseAndSetIfChanged(ref _letra, value);
    }

    public IBrush Color
    {
      get => _color;
      set => this.RaiseAndSetIfChanged(ref _color, value);
    }

    public bool EstaSeleccionada
    {
      get => _estaSeleccionada;
      set => this.RaiseAndSetIfChanged(ref _estaSeleccionada, value);
    }

    public int Fila { get; set; }
    public int Columna { get; set; }
  }
}