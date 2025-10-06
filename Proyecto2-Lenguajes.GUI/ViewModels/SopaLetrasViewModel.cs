using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using Proyecto2_Lenguajes.Logic.SopaLetras;
using System.IO;

namespace Proyecto2_Lenguajes.GUI.ViewModels;

public class SopaLetrasViewModel : ViewModelBase
{
    private readonly Window _window;
    private Types.SopaLetras? _sopaActual;
    private ObservableCollection<ObservableCollection<CeldaViewModel>> _filasMatriz;
    private ObservableCollection<string> _palabrasEnSopa;
    private ObservableCollection<string> _palabrasEncontradas;
    private string _estadoJuego;
    private CeldaViewModel? _celdaInicio;
    private CeldaViewModel? _celdaFin;
    private bool _juegoTerminado = false;

    public SopaLetrasViewModel(Window window)
    {
        _window = window;
        _filasMatriz = new ObservableCollection<ObservableCollection<CeldaViewModel>>();
        _palabrasEnSopa = new ObservableCollection<string>();
        _palabrasEncontradas = new ObservableCollection<string>();
        _estadoJuego = "Cargando juego...";

        // Comandos simples sin problemas de threading
        GenerarNuevaSopaCommand = new SimpleCommand(GenerarNuevaSopa);
        VolverCommand = new SimpleCommand(Volver);
        MostrarSolucionesCommand = new SimpleCommand(MostrarSoluciones);

        // Generar sopa inicial con palabras de ejemplo
        GenerarSopaInicial();
    }

    public ObservableCollection<ObservableCollection<CeldaViewModel>> FilasMatriz
    {
        get => _filasMatriz;
        set => this.RaiseAndSetIfChanged(ref _filasMatriz, value);
    }

    public ObservableCollection<string> PalabrasEnSopa
    {
        get => _palabrasEnSopa;
        set => this.RaiseAndSetIfChanged(ref _palabrasEnSopa, value);
    }

    public ObservableCollection<string> PalabrasEncontradas
    {
        get => _palabrasEncontradas;
        set => this.RaiseAndSetIfChanged(ref _palabrasEncontradas, value);
    }

    public string EstadoJuego
    {
        get => _estadoJuego;
        set => this.RaiseAndSetIfChanged(ref _estadoJuego, value);
    }

    public ICommand GenerarNuevaSopaCommand { get; }
    public ICommand VolverCommand { get; }
    public ICommand MostrarSolucionesCommand { get; }

    private void GenerarSopaInicial()
    {
        string rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "palabras.txt");
        var lineasOriginales = File.ReadAllLines(rutaArchivo);
        List<string> palabrasArchivo = ReadFile.procesarLineas(lineasOriginales.ToArray()).ToList();

        // Informar si se filtraron palabras por longitud
        int palabrasOriginales = lineasOriginales.Count(l => !string.IsNullOrWhiteSpace(l));
        int palabrasDescartadas = palabrasOriginales - palabrasArchivo.Count;

        _palabrasEnSopa = new ObservableCollection<string>(palabrasArchivo);
        _palabrasEncontradas = new ObservableCollection<string>();

        var palabrasFSharp = Microsoft.FSharp.Collections.ListModule.OfSeq(palabrasArchivo);
        var seed = DateTime.Now.Millisecond;

        _sopaActual = Generador.generarSopaLetras(palabrasFSharp, seed);

        ActualizarMatriz();

        if (palabrasDescartadas > 0)
        {
            EstadoJuego = $"Sopa generada con {palabrasArchivo.Count} palabras ({palabrasDescartadas} descartadas por exceder longitud máxima de {ReadFile.longitudMaximaPalabra}).";
        }
        else
        {
            EstadoJuego = $"Sopa generada con {palabrasArchivo.Count} palabras. Selecciona inicio y fin de palabra.";
        }
    }

    private void ActualizarMatriz()
    {
        if (_sopaActual == null) return;

        var nuevasFilas = new ObservableCollection<ObservableCollection<CeldaViewModel>>();
        var tamaño = _sopaActual.Tamaño;

        for (int fila = 0; fila < tamaño; fila++)
        {
            var columnas = new ObservableCollection<CeldaViewModel>();
            for (int col = 0; col < tamaño; col++)
            {
                char caracter = _sopaActual.Matriz[fila, col];
                var celda = new CeldaViewModel
                {
                    Letra = caracter,
                    Fila = fila,
                    Columna = col
                };

                columnas.Add(celda);
            }
            nuevasFilas.Add(columnas);
        }

        FilasMatriz = nuevasFilas;
    }

    public void ManejadorCeldaClick(CeldaViewModel celda)
    {
        if (_juegoTerminado)
        {
            EstadoJuego = "Esta partida ha finalizado. Genera una nueva sopa para seguir jugando.";
            return;
        }
        if (_celdaInicio == null)
        {
            // Seleccionar inicio
            _celdaInicio = celda;
            celda.ColorTemporal = Brushes.Yellow;
            celda.EstaSeleccionada = true;

            EstadoJuego = $"Inicio: ({celda.Fila}, {celda.Columna}). Ahora selecciona el final de la palabra.";
        }
        else if (_celdaFin == null)
        {
            // Seleccionar fin y validar
            _celdaFin = celda;
            ValidarSeleccion();
        }
        else
        {
            // Ya hay una selección completa, resetear y empezar de nuevo
            if (_celdaInicio != null && !_celdaInicio.EstaSeleccionada)
            {
                _celdaInicio.ColorTemporal = Brushes.White;
            }

            _celdaInicio = celda;
            _celdaFin = null;
            celda.ColorTemporal = Brushes.Yellow;
            celda.EstaSeleccionada = true;

            EstadoJuego = $"Inicio: ({celda.Fila}, {celda.Columna}). Ahora selecciona el final de la palabra.";
        }
    }

    private void ValidarSeleccion()
    {
        if (_celdaInicio == null || _celdaFin == null || _sopaActual == null)
            return;

        // Crear posiciones para F#
        var inicio = new Types.Posicion(_celdaInicio.Fila, _celdaInicio.Columna);
        var fin = new Types.Posicion(_celdaFin.Fila, _celdaFin.Columna);

        // Llamar a la función de validación de F#
        var resultado = Validations.verificarSeleccion(_sopaActual, Microsoft.FSharp.Collections.ListModule.OfSeq(_palabrasEncontradas), inicio, fin);

        if (resultado != null)
        {
            // ¡Palabra encontrada!
            var palabraEncontrada = resultado.Value;

            // Marcar la palabra como encontrada
            _sopaActual = Validations.marcarPalabraEncontrada(_sopaActual, palabraEncontrada);

            // Añadir a la lista de palabras encontradas
            if (!_palabrasEncontradas.Contains(palabraEncontrada.Palabra.ToUpper()))
            {
                _palabrasEncontradas.Add(palabraEncontrada.Palabra.ToUpper());
                _palabrasEnSopa.Remove(palabraEncontrada.Palabra.ToUpper());
            }

            // Colorear el camino
            ColorearCamino(inicio, fin, palabraEncontrada.Direccion, Brushes.LightGreen);

            var progreso = Validations.obtenerProgreso(_sopaActual);
            EstadoJuego = $"¡Correcto! Encontraste '{palabraEncontrada.Palabra}'. Progreso: {progreso:F1}%";

            // Verificar si completó el juego
            if (Validations.juegoCompletado(_sopaActual))
            {
                _juegoTerminado = true;
                EstadoJuego = "¡FELICIDADES! Completaste la sopa de letras.";
            }
        }
        else
        {
            // Selección incorrecta
            EstadoJuego = "Selección incorrecta. Intenta de nuevo.";

            // Resetear colores temporales
            if (_celdaInicio != null)
            {
                if (_celdaInicio.EsParteDePalabra)
                {
                    _celdaInicio.ColorTemporal = Brushes.LightGreen;
                }
                else
                {
                    _celdaInicio.ColorTemporal = Brushes.White;
                }
                _celdaInicio.EstaSeleccionada = false;
            }
        }

        // Limpiar selección
        if (_celdaInicio != null && resultado == null)
        {
            if (_celdaInicio.EsParteDePalabra)
            {
                _celdaInicio.ColorTemporal = Brushes.LightGreen;
            }
            else
            {
                _celdaInicio.ColorTemporal = Brushes.White;
            }
            _celdaInicio.EstaSeleccionada = false;
        }

        _celdaInicio = null;
        _celdaFin = null;
    }

    private void ColorearCamino(Types.Posicion inicio, Types.Posicion fin, Types.Direccion direccion, IBrush color)
    {
        var posActual = inicio;

        while (true)
        {
            var celda = FilasMatriz[posActual.Fila][posActual.Columna];
            celda.Color = color;
            celda.ColorTemporal = color;
            celda.EstaSeleccionada = false;
            celda.EsParteDePalabra = true;

            if (posActual.Fila == fin.Fila && posActual.Columna == fin.Columna)
                break;

            posActual = Validations.moverPosicion(posActual, direccion);
        }
    }

    private void MostrarSoluciones()
    {
        if (_sopaActual == null) return;

        // Usar la función de F# para encontrar todas las soluciones
        var soluciones = BusquedaPalabras.encontrarTodasSoluciones(_sopaActual, Microsoft.FSharp.Collections.ListModule.OfSeq(_palabrasEncontradas));

        foreach (var solucion in soluciones)
        {
            // Colorear en azul claro las soluciones automáticas

            ColorearCamino(solucion.Inicio, solucion.Fin, solucion.Direccion, Brushes.LightBlue);
        }

        var cantidadSoluciones = Microsoft.FSharp.Collections.ListModule.Length(soluciones);
        EstadoJuego = $"Se encontraron {cantidadSoluciones} palabras automáticamente (en azul claro).";
        _juegoTerminado = true;
    }

    private void GenerarNuevaSopa()
    {
        if (_sopaActual == null) return;

        var palabras = _sopaActual.Palabras;
        var nuevoSeed = DateTime.Now.Millisecond;

        _sopaActual = Generador.generarSopaLetras(palabras, nuevoSeed);

        // Reincorporar palabras encontradas a la lista de palabras disponibles
        foreach (var palabra in _palabrasEncontradas)
        {
            if (!_palabrasEnSopa.Contains(palabra))
            {
                _palabrasEnSopa.Add(palabra);
            }
        }

        _palabrasEncontradas.Clear();

        ActualizarMatriz();

        var cantidadPalabras = Microsoft.FSharp.Collections.ListModule.Length(palabras);
        EstadoJuego = $"Nueva sopa generada con {cantidadPalabras} palabras";
        _juegoTerminado = false;
    }

    private void Volver()
    {
        _window.Close();
    }
}