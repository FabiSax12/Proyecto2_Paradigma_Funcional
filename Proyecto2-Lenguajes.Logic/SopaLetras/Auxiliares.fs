namespace Proyecto2_Lenguajes.Logic.SopaLetras

open Types

module Auxiliares =
/// Obtiene el delta de movimiento para una dirección
    let obtenerDelta (direccion: Direccion) =
        match direccion with
        | Horizontal -> (0, 1)
        | HorizontalInversa -> (0, -1)
        | Vertical -> (1, 0)
        | VerticalInversa -> (-1, 0)
        | DiagonalDerecha -> (1, 1)
        | DiagonalDerechaInversa -> (-1, -1)
        | DiagonalIzquierda -> (1, -1)
        | DiagonalIzquierdaInversa -> (-1, 1)

    /// Verifica si una posición está dentro de los límites de la matriz
    let posicionValida (tamaño: int) (pos: Posicion) =
        pos.Fila >= 0 && pos.Fila < tamaño &&
        pos.Columna >= 0 && pos.Columna < tamaño

    /// Obtiene el carácter en una posición de la matriz
    let obtenerCaracter (matriz: char[,]) (pos: Posicion) =
        if posicionValida (Array2D.length1 matriz) pos then
            Some matriz.[pos.Fila, pos.Columna]
        else
            None

    /// Mueve una posición en una dirección específica
    let moverPosicion pos direccion =
        let (deltaFila, deltaCol) = obtenerDelta direccion
        { Fila = pos.Fila + deltaFila; Columna = pos.Columna + deltaCol }

    /// Obtiene todas las direcciones posibles
    let todasDirecciones = [
        Horizontal; HorizontalInversa; Vertical; VerticalInversa;
        DiagonalDerecha; DiagonalDerechaInversa;
        DiagonalIzquierda; DiagonalIzquierdaInversa
    ]