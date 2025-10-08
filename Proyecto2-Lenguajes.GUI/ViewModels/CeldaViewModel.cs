using Avalonia.Media;
using ReactiveUI;

namespace Proyecto2_Lenguajes.GUI.ViewModels
{
    public class CeldaViewModel : ViewModelBase
    {
        private char _letra;
        private IBrush _color = Brushes.White;
        private IBrush _colorTemporal = Brushes.White;
        private bool _estaSeleccionada;
        private bool _esParteDePalabra;
        private bool _encontradaPorAlgoritmo;

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

        public IBrush ColorTemporal
        {
            get => _colorTemporal;
            set
            {
                this.RaiseAndSetIfChanged(ref _colorTemporal, value);
                if (!EstaSeleccionada)
                {
                    Color = value;
                }
            }
        }

        public bool EstaSeleccionada
        {
            get => _estaSeleccionada;
            set => this.RaiseAndSetIfChanged(ref _estaSeleccionada, value);
        }

        public bool EsParteDePalabra
        {
            get => _esParteDePalabra;
            set => this.RaiseAndSetIfChanged(ref _esParteDePalabra, value);
        }

        public bool EncontradaPorAlgoritmo
        {
            get => _encontradaPorAlgoritmo;
            set => this.RaiseAndSetIfChanged(ref _encontradaPorAlgoritmo, value);
        }

        public int Fila { get; set; }
        public int Columna { get; set; }
    }
}