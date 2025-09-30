using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Proyecto2_Lenguajes.GUI.ViewModels;
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

        public SopaLetrasViewModel()
        {
            _matriz = new ObservableCollection<ObservableCollection<CeldaViewModel>>();
            _palabrasPorEncontrar = new ObservableCollection<string>();
            _palabrasEncontradas = new ObservableCollection<string>();
            _colorMensaje = Brushes.Black;
        }

        // ===== ACTUALIZAR MATRIZ =====
        /// <summary>
        /// Actualiza la UI con el estado de la sopa de letras
        /// </summary>
        public void ActualizarMatriz(SopaLetras.Estado estado)
        {
            _estadoActual = estado;

            // Limpiar matriz actual
            Matriz.Clear();

            int filas = estado.Matriz.GetLength(0);
            int columnas = estado.Matriz.GetLength(1);

            // Crear celdas para la UI
            for (int i = 0; i < filas; i++)
            {
                var fila = new ObservableCollection<CeldaViewModel>();
                for (int j = 0; j < columnas; j++)
                {
                    fila.Add(new CeldaViewModel
                    {
                        Letra = estado.Matriz[i, j],
                        Fila = i,
                        Columna = j,
                        Color = Brushes.White,
                        EstaSeleccionada = false
                    });
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

                // Marcar las palabras encontradas automáticamente
                MarcarPalabraEnMatriz(
                    palabra.Inicio.Fila,
                    palabra.Inicio.Columna,
                    palabra.Fin.Fila,
                    palabra.Fin.Columna,
                    Brushes.LightGreen  // Color diferente para auto-encontradas
                );
            }
        }

        // ===== VALIDAR Y MARCAR SELECCIÓN DEL USUARIO =====
        /// <summary>
        /// Valida la selección del usuario y marca la palabra si es correcta
        /// </summary>
        public void ValidarYMarcarSeleccion(int filaInicio, int colInicio, int filaFin, int colFin)
        {
            if (_estadoActual == null) return;

            // Llamar a función F# pura para validación
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
                // Palabra correcta encontrada por el usuario
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
                // Selección incorrecta
                MostrarError();
            }
        }

        // ===== MOSTRAR PALABRA ENCONTRADA =====
        /// <summary>
        /// Muestra feedback visual cuando el usuario encuentra una palabra correcta
        /// </summary>
        private void MostrarPalabraEncontrada(
            string palabra,
            int filaInicio,
            int colInicio,
            int filaFin,
            int colFin)
        {
            // Marcar palabra en la matriz con color de usuario
            MarcarPalabraEnMatriz(
                filaInicio,
                colInicio,
                filaFin,
                colFin,
                Brushes.LightBlue  // Color para palabras encontradas por usuario
            );

            // Mover de "por encontrar" a "encontradas"
            PalabrasPorEncontrar.Remove(palabra);
            if (!PalabrasEncontradas.Contains(palabra))
            {
                PalabrasEncontradas.Add(palabra);
            }

            // Mostrar mensaje de éxito
            MensajeEstado = $"¡Correcto! Encontraste: {palabra.ToUpper()}";
            ColorMensaje = Brushes.Green;

            // Verificar si ganó
            if (PalabrasPorEncontrar.Count == 0)
            {
                MensajeEstado = "🎉 ¡FELICITACIONES! Encontraste todas las palabras";
                ColorMensaje = Brushes.DarkGreen;
            }
        }

        // ===== MOSTRAR ERROR =====
        /// <summary>
        /// Muestra feedback visual cuando el usuario hace una selección incorrecta
        /// </summary>
        private void MostrarError()
        {
            MensajeEstado = "❌ Esa palabra no está en la lista o ya fue encontrada";
            ColorMensaje = Brushes.Red;

            // Opcional: hacer que el mensaje desaparezca después de 2 segundos
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
            {
                MensajeEstado = "";
                ColorMensaje = Brushes.Black;
            });
        }

        // ===== MARCAR PALABRA EN MATRIZ (HELPER) =====
        /// <summary>
        /// Marca visualmente una palabra en la matriz
        /// </summary>
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

        // ===== CARGAR JUEGO DESDE ARCHIVO =====
        /// <summary>
        /// Carga palabras desde archivo y genera la sopa de letras
        /// </summary>
        public void CargarJuego(string rutaArchivo)
        {
            try
            {
                // Leer palabras del archivo
                var palabras = File.ReadAllLines(rutaArchivo)
                    .Where(linea => !string.IsNullOrWhiteSpace(linea))
                    .Select(linea => linea.Trim().ToUpper())
                    .ToList();

                if (palabras.Count == 0)
                {
                    MostrarError();
                    MensajeEstado = "❌ El archivo no contiene palabras válidas";
                    return;
                }

                // Generar matriz (aquí necesitarías tu función generadora en F#)
                // Por ahora, asumiendo que tienes una función GenerarMatriz
                var matriz = SopaLetras.GenerarMatriz(
                    Microsoft.FSharp.Collections.ListModule.OfSeq(palabras),
                    15,  // tamaño de la matriz
                    15
                );

                // Resolver automáticamente (encuentra todas las palabras)
                var estado = SopaLetras.resolverAutomatico(matriz,
                    Microsoft.FSharp.Collections.ListModule.OfSeq(palabras));

                // Actualizar UI
                ActualizarMatriz(estado);

                MensajeEstado = $"Juego cargado. Encuentra {palabras.Count} palabras";
                ColorMensaje = Brushes.Blue;
            }
            catch (Exception ex)
            {
                MensajeEstado = $"❌ Error al cargar archivo: {ex.Message}";
                ColorMensaje = Brushes.Red;
            }
        }
    }
}