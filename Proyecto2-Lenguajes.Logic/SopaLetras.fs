module Proyecto2_Lenguajes.Logic.SopaLetras

// Tipos inmutables
type Celda = {
  Fila: int;
  Columna: int;
  Letra: char
}
type Posicion = { Fila: int; Columna: int }
type Camino = Celda list

type Direccion =
    | Horizontal
    | Vertical
    | DiagonalDerecha
    | DiagonalIzquierda

type PalabraEncontrada = {
    Palabra: string
    Inicio: Posicion
    Fin: Posicion
    Direccion: Direccion
}

type Estado = {
    Matriz: char[,]
    PalabrasObjetivo: string list
    PalabrasEncontradas: PalabraEncontrada list
}

// Función pura - sin side effects
let vecinos (estado: Estado) (celda: Celda) : (Celda * Direccion) list =
    // Retorna vecinos válidos sin mutar estado
    []

let extender (estado: Estado) (camino: Camino) : Camino list =
    // Extiende el camino sin side effects
    []

let rec profundidad (estado: Estado) (objetivo: string) : Estado option =
    // Búsqueda en profundidad pura
    None

// Función para resolver automáticamente
let resolverAutomatico (matriz: char[,]) (palabras: string list) : Estado =
    // Retorna nuevo estado con todas las soluciones
    { Matriz = matriz; PalabrasObjetivo = palabras; PalabrasEncontradas = [] }

let validarSeleccion
    (estado: Estado)
    (filaInicio: int)
    (colInicio: int)
    (filaFin: int)
    (colFin: int)
    : (bool * string option) =

    // Return de mentiras
    (true, None)

let GenerarMatriz
    (palabras: List<string>)
    (width: int)
    (height: int)
    : char[,] =
    Array2D.create height width 'a'