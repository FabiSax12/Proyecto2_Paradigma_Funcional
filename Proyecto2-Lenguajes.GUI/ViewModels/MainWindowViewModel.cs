using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Proyecto2_Lenguajes.GUI.Views;

namespace Proyecto2_Lenguajes.GUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            AhorcadoCommand = new SimpleCommand(AbrirAhorcado);
            SopaLetrasCommand = new SimpleCommand(AbrirSopaLetras);
            SalirCommand = new SimpleCommand(CerrarJuego);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AhorcadoCommand { get; }
        public ICommand SopaLetrasCommand { get; }
        public ICommand SalirCommand { get; }

        private void AbrirAhorcado()
        {
            // TODO: Implementar ventana de Ahorcado
            // Por ahora no hacer nada, solo un placeholder
        }

        private void AbrirSopaLetras()
        {
            var sopaLetrasWindow = new SopaLetrasWindow();
            sopaLetrasWindow.Show();
        }

        private void CerrarJuego()
        {
            Environment.Exit(0);
        }
    }

    // Implementación simple de ICommand sin ReactiveUI
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        public event EventHandler? CanExecuteChanged;

        public SimpleCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
    }
}