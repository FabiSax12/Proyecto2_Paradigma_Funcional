using System;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Proyecto2_Lenguajes.Logic;
using System.Diagnostics;
using Ahorcado = Proyecto2_Lenguajes.Logic.Ahorcado;

namespace Proyecto2_Lenguajes.GUI.ViewModels;

public partial class AhorcadoViewModel : ObservableObject
{
    [ObservableProperty]
    private string _palabraMostrada = "";

    [ObservableProperty]
    private string _letrasUsadas = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErroresCometidos))]
    private int _intentosRestantes;

    [ObservableProperty]
    private string _mensajeFinal = "";

    [ObservableProperty]
    private bool _juegoActivo = true;
    
    private Ahorcado.EstadoJuego _estadoActual = null!;
    private readonly string[] _palabras;
    private const int MAX_INTENTOS = 6; // Cabeza, cuerpo, 2 brazos, 2 piernas
    private readonly Stopwatch _cronometro = new();

    public int ErroresCometidos => MAX_INTENTOS - IntentosRestantes;

    public AhorcadoViewModel()
    {
        var rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "palabras.txt");
        _palabras = File.ReadAllLines(rutaArchivo).Where(p => !string.IsNullOrWhiteSpace(p) && p.Length > 2).ToArray();
        
        NuevoJuegoCommand.Execute(null);
    }

    [RelayCommand]
    private void NuevoJuego()
    {
        _cronometro.Restart();
        var palabrasFSharp = Microsoft.FSharp.Collections.ListModule.OfSeq(_palabras);
        _estadoActual = Ahorcado.iniciarJuego(palabrasFSharp, MAX_INTENTOS);
        
        ActualizarPropiedadesUI();
        
        MensajeFinal = "";
        JuegoActivo = true;
    }

    [RelayCommand]
    private void IntentarLetra(string? letraIngresada)
    {
        if (!JuegoActivo || string.IsNullOrEmpty(letraIngresada) || !char.IsLetter(letraIngresada[0]))
        {
            return;
        }

        char letra = letraIngresada.ToUpper()[0];
        
        // No hacer nada si la letra ya fue usada
        if (_estadoActual.LetrasAdivinadas.Contains(letra)) return;

        _estadoActual = Ahorcado.intentarLetra(letra, _estadoActual);
        
        ActualizarPropiedadesUI();
        VerificarEstadoJuego();
    }

    private void ActualizarPropiedadesUI()
    {
        PalabraMostrada = Ahorcado.palabraVisible(_estadoActual);
        IntentosRestantes = _estadoActual.IntentosRestantes;
        LetrasUsadas = string.Join(", ", _estadoActual.LetrasAdivinadas.OrderBy(c => c));
    }

    private void VerificarEstadoJuego()
    {
        var tiempo = _cronometro.Elapsed.TotalSeconds;
        var resultado = Ahorcado.verificarEstado(_estadoActual, tiempo);

        if (resultado is Ahorcado.Resultado.Victoria victoria)
        {
            _cronometro.Stop();
            MensajeFinal = $"¡GANASTE! Tiempo: {victoria.Item.Tiempo:F2}s";
            JuegoActivo = false;
        }
        else if (resultado.IsDerrota)
        {
            _cronometro.Stop();
            MensajeFinal = $"¡PERDISTE! La palabra era: {_estadoActual.PalabraSecreta}";
            JuegoActivo = false;
        }
    }
}
