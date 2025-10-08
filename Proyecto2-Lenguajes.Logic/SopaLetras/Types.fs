namespace Proyecto2_Lenguajes.Logic.SopaLetras

module Types =

    type Posicion = { Fila: int; Columna: int }
    
    /// Representa las 8 direcciones posibles en la sopa de letras
    type Direccion = 
        | Horizontal          // Izquierda a derecha →
        | HorizontalInversa   // Derecha a izquierda ←
        | Vertical            // Arriba a abajo ↓
        | VerticalInversa     // Abajo a arriba ↑
        | DiagonalDerecha     // Diagonal ↘
        | DiagonalDerechaInversa // Diagonal ↖
        | DiagonalIzquierda   // Diagonal ↙
        | DiagonalIzquierdaInversa // Diagonal ↗
    
    /// Representa una palabra encontrada con su ubicación
    type PalabraEncontrada = {
        Palabra: string
        Inicio: Posicion
        Fin: Posicion
        Direccion: Direccion
        EncontradaPorUsuario: bool
    }
    
    /// Representa el estado de la sopa de letras
    type SopaLetras = {
        Matriz: char[,]
        Tamaño: int
        Palabras: string list
        PalabrasEncontradas: PalabraEncontrada list
    }
    
    /// Estado para la búsqueda en profundidad
    type EstadoBusqueda = {
        Posicion: Posicion
        Camino: Posicion list
        Palabra: string
        IndiceActual: int
        Direccion: Direccion option
    }