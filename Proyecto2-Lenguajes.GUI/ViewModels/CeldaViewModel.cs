using System;
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

        public int Fila { get; set; }
        public int Columna { get; set; }

        // Eventos para manejar la selección
        public event Action<CeldaViewModel>? OnCeldaPresionada;
        public event Action<CeldaViewModel>? OnCeldaSoltada;

        public void NotificarPresion()
        {
            OnCeldaPresionada?.Invoke(this);
        }

        public void NotificarSoltado()
        {
            OnCeldaSoltada?.Invoke(this);
        }
    }
}