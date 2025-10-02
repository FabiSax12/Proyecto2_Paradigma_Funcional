using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Proyecto2_Lenguajes.GUI.ViewModels;

namespace Proyecto2_Lenguajes.GUI.Views
{
    public partial class SopaLetrasWindow : Window
    {
        private CeldaViewModel? _celdaInicio;
        private bool _seleccionando = false;

        public SopaLetrasWindow()
        {
            InitializeComponent();

            // Configurar DataContext si no está configurado
            if (DataContext == null)
            {
                DataContext = new SopaLetrasViewModel();
            }

            // Suscribirse a eventos de la ventana para manejar selección
            this.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            this.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            this.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var celda = GetCeldaViewModel(e.Source);
            if (celda != null)
            {
                _celdaInicio = celda;
                _seleccionando = true;

                // Resaltar celda inicial
                if (celda.Color == Avalonia.Media.Brushes.White)
                {
                    celda.ColorTemporal = Avalonia.Media.Brushes.LightYellow;
                }
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_seleccionando && _celdaInicio != null)
            {
                var celdaActual = GetCeldaViewModel(e.Source);
                if (celdaActual != null)
                {
                    // Resaltar el camino entre inicio y actual
                    ResaltarCamino(_celdaInicio, celdaActual);
                }
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_seleccionando && _celdaInicio != null)
            {
                var celdaFin = GetCeldaViewModel(e.Source);

                if (celdaFin != null && DataContext is SopaLetrasViewModel viewModel)
                {
                    // Validar selección
                    viewModel.ValidarYMarcarSeleccion(
                        _celdaInicio.Fila,
                        _celdaInicio.Columna,
                        celdaFin.Fila,
                        celdaFin.Columna
                    );
                }

                // Limpiar resaltado temporal
                LimpiarResaltadoTemporal();
                _celdaInicio = null;
                _seleccionando = false;
            }
        }

        private CeldaViewModel? GetCeldaViewModel(object? source)
        {
            if (source is Control control)
            {
                // Buscar el DataContext que sea CeldaViewModel
                var current = control;
                while (current != null)
                {
                    if (current.DataContext is CeldaViewModel celda)
                    {
                        return celda;
                    }
                    current = current.Parent as Control;
                }
            }
            return null;
        }

        private void ResaltarCamino(CeldaViewModel inicio, CeldaViewModel fin)
        {
            if (DataContext is not SopaLetrasViewModel viewModel) return;

            // Limpiar resaltado previo
            LimpiarResaltadoTemporal();

            // Calcular dirección
            int deltaFila = fin.Fila - inicio.Fila;
            int deltaCol = fin.Columna - inicio.Columna;
            int pasos = System.Math.Max(System.Math.Abs(deltaFila), System.Math.Abs(deltaCol));

            if (pasos == 0) return;

            int dirFila = deltaFila == 0 ? 0 : deltaFila / System.Math.Abs(deltaFila);
            int dirCol = deltaCol == 0 ? 0 : deltaCol / System.Math.Abs(deltaCol);

            // Resaltar celdas en el camino
            for (int i = 0; i <= pasos; i++)
            {
                int fila = inicio.Fila + i * dirFila;
                int col = inicio.Columna + i * dirCol;

                if (fila >= 0 && fila < viewModel.Matriz.Count &&
                    col >= 0 && col < viewModel.Matriz[fila].Count)
                {
                    var celda = viewModel.Matriz[fila][col];
                    if (!celda.EstaSeleccionada)
                    {
                        celda.ColorTemporal = Avalonia.Media.Brushes.LightYellow;
                    }
                }
            }
        }

        private void LimpiarResaltadoTemporal()
        {
            if (DataContext is not SopaLetrasViewModel viewModel) return;

            foreach (var fila in viewModel.Matriz)
            {
                foreach (var celda in fila)
                {
                    if (!celda.EstaSeleccionada)
                    {
                        celda.Color = Avalonia.Media.Brushes.White;
                        celda.ColorTemporal = Avalonia.Media.Brushes.White;
                    }
                }
            }
        }
    }
}