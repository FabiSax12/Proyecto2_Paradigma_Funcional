using System;
using System.ComponentModel;
using System.Windows.Input;
using Proyecto2_Lenguajes.Logic;

namespace Proyecto2_Lenguajes.GUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _firstNumber = "";
        private string _secondNumber = "";
        private int _selectedOperation = 0;
        private string _result = "El resultado aparecerá aquí";

        public MainWindowViewModel()
        {
            CalculateCommand = new SimpleCommand(Calculate);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string FirstNumber
        {
            get => _firstNumber;
            set
            {
                _firstNumber = value;
                OnPropertyChanged(nameof(FirstNumber));
            }
        }

        public string SecondNumber
        {
            get => _secondNumber;
            set
            {
                _secondNumber = value;
                OnPropertyChanged(nameof(SecondNumber));
            }
        }

        public int SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                _selectedOperation = value;
                OnPropertyChanged(nameof(SelectedOperation));
            }
        }

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public ICommand CalculateCommand { get; }

        private void Calculate()
        {
            try
            {
                // Validar que se ingresaron números
                if (!double.TryParse(FirstNumber, out double first))
                {
                    Result = "❌ El primer número no es válido";
                    return;
                }

                if (!double.TryParse(SecondNumber, out double second))
                {
                    Result = "❌ El segundo número no es válido";
                    return;
                }

                // Realizar la operación usando las funciones de F#
                double resultado;
                string operacionTexto;

                switch (SelectedOperation)
                {
                    case 0: // Suma
                        resultado = Calculator.add(first, second);
                        operacionTexto = "+";
                        break;
                    case 1: // Resta
                        resultado = Calculator.subtract(first, second);
                        operacionTexto = "-";
                        break;
                    case 2: // Multiplicación
                        resultado = Calculator.multiply(first, second);
                        operacionTexto = "×";
                        break;
                    case 3: // División
                        var divisionResult = Calculator.divide(first, second);
                        if (divisionResult == null)
                        {
                            Result = "❌ No se puede dividir por cero";
                            return;
                        }
                        resultado = divisionResult.Value;
                        operacionTexto = "÷";
                        break;
                    default:
                        Result = "❌ Selecciona una operación válida";
                        return;
                }

                // Mostrar el resultado
                Result = $"✅ {first} {operacionTexto} {second} = {resultado}";
            }
            catch (Exception ex)
            {
                Result = $"❌ Error: {ex.Message}";
            }
        }
    }

    // Implementación simple de ICommand sin ReactiveUI
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;

        public SimpleCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}