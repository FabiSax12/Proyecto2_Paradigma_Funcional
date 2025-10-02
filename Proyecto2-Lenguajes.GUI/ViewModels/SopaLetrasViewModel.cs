using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using Proyecto2_Lenguajes.Logic;
using ReactiveUI;

namespace Proyecto2_Lenguajes.GUI.ViewModels
{
    public class SopaLetrasViewModel : ViewModelBase
    {
        private SopaLetras.Estado? _estadoActual;
        private ObservableCollection<ObservableCollection<CeldaViewModel>> _matriz;
        private ObservableCollection<string> _palabrasPorEncontrar;
        private ObservableCollection<string> _palabrasEncontradas;
        private string _mensajeEstado = string.Empty;
        private IBrush _colorMensaje;

        // Para la selección del usuario
        private CeldaViewModel? _celdaInicio;
        private bool _seleccionando;

        public ObservableCollection<ObservableCollection<CeldaViewModel>> Matriz
        {
            get => _matriz;
            set => this.RaiseAndSetIfChanged(ref _matriz, value);
        }

        public ObservableCollection<string> PalabrasPorEncontrar
        {
            get => _palabrasPorEncontrar;
            set => this.RaiseAndSetIfChanged(ref _palabrasPorEncontrar, value);
        }

        public ObservableCollection<string> PalabrasEncontradas
        {
            get => _palabrasEncontradas;
            set => this.RaiseAndSetIfChanged(ref _palabrasEncontradas, value);
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => this.RaiseAndSetIfChanged(ref _mensajeEstado, value);
        }

        public IBrush ColorMensaje
        {
            get => _colorMensaje;
            set => this.RaiseAndSetIfChanged(ref _colorMensaje, value);
        }

        // Comandos
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CargarJuegoFacilCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CargarJuegoMedioCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CargarJuegoDificilCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CargarPorTemaCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ResolverAutomaticoCommand { get; }
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ReiniciarCommand { get; }

        public SopaLetrasViewModel()
        {
            _matriz = new ObservableCollection<ObservableCollection<CeldaViewModel>>();
            _palabrasPorEncontrar = new ObservableCollection<string>();
            _palabrasEncontradas = new ObservableCollection<string>();
            _colorMensaje = Brushes.Black;

            // Inicializar comandos con diferentes dificultades
            CargarJuegoFacilCommand = ReactiveCommand.Create(() => CargarJuegoPorDificultad("FACIL"));
            CargarJuegoMedioCommand = ReactiveCommand.Create(() => CargarJuegoPorDificultad("MEDIO"));
            CargarJuegoDificilCommand = ReactiveCommand.Create(() => CargarJuegoPorDificultad("DIFICIL"));
            CargarPorTemaCommand = ReactiveCommand.Create(() => CargarJuegoPorTema("ANIMALES"));
            ResolverAutomaticoCommand = ReactiveCommand.Create(ResolverAutomatico);
            ReiniciarCommand = ReactiveCommand.Create(Reiniciar);

            // Mensaje inicial
            MensajeEstado = "🎮 Selecciona un nivel de dificultad para comenzar";
            ColorMensaje = Brushes.Gray;
        }

        // ===== ACTUALIZAR MATRIZ =====
        public void ActualizarMatriz(SopaLetras.Estado estado)
        {
            _estadoActual = estado;
            Matriz.Clear();

            int filas = estado.Matriz.GetLength(0);
            int columnas = estado.Matriz.GetLength(1);

            for (int i = 0; i < filas; i++)
            {
                var fila = new ObservableCollection<CeldaViewModel>();
                for (int j = 0; j < columnas; j++)
                {
                    var celda = new CeldaViewModel
                    {
                        Letra = estado.Matriz[i, j],
                        Fila = i,
                        Columna = j,
                        Color = Brushes.White,
                        EstaSeleccionada = false
                    };

                    // Suscribirse a eventos de clic
                    celda.OnCeldaPresionada += CeldaPresionada;
                    celda.OnCeldaSoltada += CeldaSoltada;

                    fila.Add(celda);
                }
                Matriz.Add(fila);
            }

            // Actualizar listas de palabras
            PalabrasPorEncontrar.Clear();
            foreach (var palabra in estado.PalabrasObjetivo)
            {
                PalabrasPorEncontrar.Add(palabra);
            }

            PalabrasEncontradas.Clear();
            foreach (var palabra in estado.PalabrasEncontradas)
            {
                PalabrasEncontradas.Add(palabra.Palabra);
                MarcarPalabraEnMatriz(
                    palabra.Inicio.Fila,
                    palabra.Inicio.Columna,
                    palabra.Fin.Fila,
                    palabra.Fin.Columna,
                    Brushes.LightGreen
                );
            }
        }

        // ===== MANEJO DE SELECCIÓN DEL USUARIO =====
        private void CeldaPresionada(CeldaViewModel celda)
        {
            _celdaInicio = celda;
            _seleccionando = true;
            celda.ColorTemporal = Brushes.Yellow;
        }

        private void CeldaSoltada(CeldaViewModel celda)
        {
            if (_seleccionando && _celdaInicio != null)
            {
                _seleccionando = false;

                // Validar selección
                ValidarYMarcarSeleccion(
                    _celdaInicio.Fila,
                    _celdaInicio.Columna,
                    celda.Fila,
                    celda.Columna
                );

                // Limpiar resaltado temporal
                LimpiarSeleccionTemporal();
                _celdaInicio = null;
            }
        }

        private void LimpiarSeleccionTemporal()
        {
            foreach (var fila in Matriz)
            {
                foreach (var celda in fila)
                {
                    if (!celda.EstaSeleccionada)
                    {
                        celda.Color = Brushes.White;
                    }
                }
            }
        }

        // ===== VALIDAR Y MARCAR SELECCIÓN =====
        public void ValidarYMarcarSeleccion(int filaInicio, int colInicio, int filaFin, int colFin)
        {
            if (_estadoActual == null) return;

            var resultado = SopaLetras.validarSeleccion(
                _estadoActual,
                filaInicio,
                colInicio,
                filaFin,
                colFin
            );

            bool esValida = resultado.Item1;
            var palabraEncontrada = resultado.Item2;

            if (esValida && palabraEncontrada != null)
            {
                MostrarPalabraEncontrada(
                    palabraEncontrada.Value,
                    filaInicio,
                    colInicio,
                    filaFin,
                    colFin
                );
            }
            else
            {
                MostrarError();
            }
        }

        private void MostrarPalabraEncontrada(
            string palabra,
            int filaInicio,
            int colInicio,
            int filaFin,
            int colFin)
        {
            MarcarPalabraEnMatriz(filaInicio, colInicio, filaFin, colFin, Brushes.LightBlue);

            PalabrasPorEncontrar.Remove(palabra);
            if (!PalabrasEncontradas.Contains(palabra))
            {
                PalabrasEncontradas.Add(palabra);
            }

            MensajeEstado = $"¡Correcto! Encontraste: {palabra.ToUpper()}";
            ColorMensaje = Brushes.Green;

            if (PalabrasPorEncontrar.Count == 0)
            {
                MensajeEstado = "🎉 ¡FELICITACIONES! Encontraste todas las palabras";
                ColorMensaje = Brushes.DarkGreen;
            }
        }

        private void MostrarError()
        {
            MensajeEstado = "❌ Esa palabra no está en la lista o ya fue encontrada";
            ColorMensaje = Brushes.Red;

            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
            {
                Dispatcher.UIThread.Post(() => {
        MensajeEstado = "";  // ✅ Se ejecuta en thread de UI
    });
                ColorMensaje = Brushes.Black;
            });
        }

        private void MarcarPalabraEnMatriz(
            int filaInicio,
            int colInicio,
            int filaFin,
            int colFin,
            IBrush color)
        {
            int deltaFila = filaFin - filaInicio;
            int deltaCol = colFin - colInicio;
            int pasos = Math.Max(Math.Abs(deltaFila), Math.Abs(deltaCol));

            int dirFila = deltaFila == 0 ? 0 : deltaFila / Math.Abs(deltaFila);
            int dirCol = deltaCol == 0 ? 0 : deltaCol / Math.Abs(deltaCol);

            for (int i = 0; i <= pasos; i++)
            {
                int fila = filaInicio + i * dirFila;
                int col = colInicio + i * dirCol;

                Matriz[fila][col].Color = color;
                Matriz[fila][col].EstaSeleccionada = true;
            }
        }

        // ===== COMANDOS =====
        private void CargarJuegoPorDificultad(string dificultad)
        {
            System.Diagnostics.Debug.WriteLine($"CargarJuegoPorDificultad llamado: {dificultad}");

            try
            {
                // Obtener palabras desde F#
                var palabrasFSharp = SopaLetras.obtenerPalabrasPorDificultad(dificultad);
                var palabras = Microsoft.FSharp.Collections.ListModule.ToArray(palabrasFSharp).ToList();

                System.Diagnostics.Debug.WriteLine($"Palabras cargadas desde F#: {palabras.Count}");

                if (palabras.Count == 0)
                {
                    MensajeEstado = "❌ No hay palabras disponibles";
                    ColorMensaje = Brushes.Red;
                    return;
                }

                // Generar matriz usando F#
                var matriz = SopaLetras.GenerarMatriz(
                    palabrasFSharp,
                    15,
                    15
                );

                // Crear estado inicial
                var estado = SopaLetras.resolverAutomatico(
                    matriz,
                    palabrasFSharp
                );

                ActualizarMatriz(estado);

                MensajeEstado = $"✅ Juego {dificultad} cargado. Encuentra {palabras.Count} palabras";
                ColorMensaje = Brushes.Blue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CargarJuegoPorDificultad: {ex.Message}");
                MensajeEstado = $"❌ Error al cargar juego: {ex.Message}";
                ColorMensaje = Brushes.Red;
            }
        }

        private void CargarJuegoPorTema(string tema)
        {
            System.Diagnostics.Debug.WriteLine($"CargarJuegoPorTema llamado: {tema}");

            try
            {
                // Obtener palabras desde F#
                var palabrasFSharp = SopaLetras.obtenerPalabrasPorTema(tema);
                var palabras = Microsoft.FSharp.Collections.ListModule.ToArray(palabrasFSharp).ToList();

                System.Diagnostics.Debug.WriteLine($"Palabras del tema {tema}: {palabras.Count}");

                if (palabras.Count == 0)
                {
                    MensajeEstado = "❌ No hay palabras disponibles para este tema";
                    ColorMensaje = Brushes.Red;
                    return;
                }

                // Generar matriz usando F#
                var matriz = SopaLetras.GenerarMatriz(
                    palabrasFSharp,
                    15,
                    15
                );

                // Crear estado inicial
                var estado = SopaLetras.resolverAutomatico(
                    matriz,
                    palabrasFSharp
                );

                ActualizarMatriz(estado);

                MensajeEstado = $"✅ Tema {tema} cargado. Encuentra {palabras.Count} palabras";
                ColorMensaje = Brushes.Blue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CargarJuegoPorTema: {ex.Message}");
                MensajeEstado = $"❌ Error al cargar tema: {ex.Message}";
                ColorMensaje = Brushes.Red;
            }
        }

        private void ResolverAutomatico()
        {
            System.Diagnostics.Debug.WriteLine("ResolverAutomatico llamado");

            if (_estadoActual == null)
            {
                MensajeEstado = "⚠️ Primero debes cargar un juego";
                ColorMensaje = Brushes.Orange;
                return;
            }

            try
            {
                var estadoResuelto = SopaLetras.resolverAutomatico(
                    _estadoActual.Matriz,
                    Microsoft.FSharp.Collections.ListModule.OfSeq(_estadoActual.PalabrasObjetivo)
                );

                ActualizarMatriz(estadoResuelto);
                MensajeEstado = "✅ Todas las palabras han sido encontradas automáticamente";
                ColorMensaje = Brushes.Blue;
            }
            catch (Exception ex)
            {
                MensajeEstado = $"❌ Error al resolver: {ex.Message}";
                ColorMensaje = Brushes.Red;
            }
        }

        private void Reiniciar()
        {
            System.Diagnostics.Debug.WriteLine("Reiniciar llamado");

            Matriz.Clear();
            PalabrasPorEncontrar.Clear();
            PalabrasEncontradas.Clear();
            _estadoActual = null;
            MensajeEstado = "🔄 Juego reiniciado. Selecciona un nivel de dificultad para comenzar";
            ColorMensaje = Brushes.Gray;
        }
    }
}