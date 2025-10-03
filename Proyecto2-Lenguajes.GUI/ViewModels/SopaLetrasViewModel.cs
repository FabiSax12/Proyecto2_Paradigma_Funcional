using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Proyecto2_Lenguajes.Logic.SopaLetras;

namespace Proyecto2_Lenguajes.GUI.ViewModels;

public class SopaLetrasViewModel : ViewModelBase
{
    private readonly Window _window;
    private Types.SopaLetras? _sopaActual;
    private ObservableCollection<ObservableCollection<string>> _filasMatriz;
    private string _estadoJuego;

    public SopaLetrasViewModel(Window window)
    {
        _window = window;
        _filasMatriz = new ObservableCollection<ObservableCollection<string>>();
        _estadoJuego = "Cargando juego...";

        // Comandos
        CargarPalabrasCommand = ReactiveCommand.CreateFromTask(CargarPalabrasAsync);
        GenerarNuevaSopaCommand = ReactiveCommand.Create(GenerarNuevaSopa);
        VolverCommand = ReactiveCommand.Create(Volver);

        // Generar sopa inicial con palabras de ejemplo
        GenerarSopaInicial();
    }

    public ObservableCollection<ObservableCollection<string>> FilasMatriz
    {
        get => _filasMatriz;
        set => this.RaiseAndSetIfChanged(ref _filasMatriz, value);
    }

    public string EstadoJuego
    {
        get => _estadoJuego;
        set => this.RaiseAndSetIfChanged(ref _estadoJuego, value);
    }

    public ICommand CargarPalabrasCommand { get; }
    public ICommand GenerarNuevaSopaCommand { get; }
    public ICommand VolverCommand { get; }

    private void GenerarSopaInicial()
    {
        // Palabras de ejemplo para empezar
        var palabrasEjemplo = new List<string>
        {
            "CASA",
            "PERRO",
            "GATO",
            "SOL",
            "LUNA",
            "ARBOL",
            "FLOR"
        };

        var palabrasFSharp = Microsoft.FSharp.Collections.ListModule.OfSeq(palabrasEjemplo);
        var seed = DateTime.Now.Millisecond;

        _sopaActual = SopaLetras.generarSopaLetras(palabrasFSharp, seed);

        ActualizarMatriz();

        EstadoJuego = $"Sopa generada con {palabrasEjemplo.Count} palabras";
    }

    private void ActualizarMatriz()
    {
        if (_sopaActual == null) return;

        var nuevasFilas = new ObservableCollection<ObservableCollection<string>>();
        var tamaño = _sopaActual.Tamaño;

        for (int fila = 0; fila < tamaño; fila++)
        {
            var columnas = new ObservableCollection<string>();
            for (int col = 0; col < tamaño; col++)
            {
                char caracter = _sopaActual.Matriz[fila, col];
                columnas.Add(caracter.ToString());
            }
            nuevasFilas.Add(columnas);
        }

        FilasMatriz = nuevasFilas;
    }

    private async Task CargarPalabrasAsync()
    {
        try
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar archivo de palabras",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Archivos de texto", Extensions = { "txt" } },
                    new FileDialogFilter { Name = "Todos los archivos", Extensions = { "*" } }
                }
            };

            var result = await openFileDialog.ShowAsync(_window);

            if (result != null && result.Length > 0)
            {
                var rutaArchivo = result[0];
                var palabras = ReadFile.leerPalabrasDesdeArchivo(rutaArchivo);

                if (Microsoft.FSharp.Collections.ListModule.IsEmpty(palabras))
                {
                    EstadoJuego = "No se pudieron cargar palabras del archivo";
                    return;
                }

                var seed = DateTime.Now.Millisecond;
                _sopaActual = SopaLetras.generarSopaLetras(palabras, seed);

                ActualizarMatriz();

                var cantidadPalabras = Microsoft.FSharp.Collections.ListModule.Length(palabras);
                EstadoJuego = $"Sopa generada con {cantidadPalabras} palabras del archivo";
            }
        }
        catch (Exception ex)
        {
            EstadoJuego = $"Error al cargar archivo: {ex.Message}";
        }
    }

    private void GenerarNuevaSopa()
    {
        if (_sopaActual == null) return;

        var palabras = _sopaActual.Palabras;
        var nuevoSeed = DateTime.Now.Millisecond;

        _sopaActual = SopaLetras.generarSopaLetras(palabras, nuevoSeed);

        ActualizarMatriz();

        var cantidadPalabras = Microsoft.FSharp.Collections.ListModule.Length(palabras);
        EstadoJuego = $"Nueva sopa generada con {cantidadPalabras} palabras";
    }

    private void Volver()
    {
        _window.Close();
    }
}
