namespace Proyecto2_Lenguajes.Logic.SopaLetras

open Types

module Validations = 

    /// Mueve una posición en la dirección especificada
    let moverPosicion (pos: Posicion) direccion =
        match direccion with
        | Horizontal -> { pos with Columna = pos.Columna + 1 }
        | HorizontalInversa -> { pos with Columna = pos.Columna - 1 }
        | Vertical -> { pos with Fila = pos.Fila + 1 }
        | VerticalInversa -> { pos with Fila = pos.Fila - 1 }
        | DiagonalDerecha -> { Fila = pos.Fila + 1; Columna = pos.Columna + 1 }
        | DiagonalDerechaInversa -> { Fila = pos.Fila - 1; Columna = pos.Columna - 1 }
        | DiagonalIzquierda -> { Fila = pos.Fila + 1; Columna = pos.Columna - 1 }
        | DiagonalIzquierdaInversa -> { Fila = pos.Fila - 1; Columna = pos.Columna + 1 }

    /// Verifica si una selección del usuario es correcta
    let verificarSeleccion (sopa: SopaLetras) inicio fin =
        let deltaFila = fin.Fila - inicio.Fila
        let deltaCol = fin.Columna - inicio.Columna
        
        // Determinar la dirección de la selección
        let direccionOpt =
            if deltaFila = 0 && deltaCol > 0 then Some Horizontal
            elif deltaFila = 0 && deltaCol < 0 then Some HorizontalInversa
            elif deltaCol = 0 && deltaFila > 0 then Some Vertical
            elif deltaCol = 0 && deltaFila < 0 then Some VerticalInversa
            elif deltaFila > 0 && deltaCol > 0 && deltaFila = deltaCol then Some DiagonalDerecha
            elif deltaFila < 0 && deltaCol < 0 && deltaFila = deltaCol then Some DiagonalDerechaInversa
            elif deltaFila > 0 && deltaCol < 0 && deltaFila = -deltaCol then Some DiagonalIzquierda
            elif deltaFila < 0 && deltaCol > 0 && -deltaFila = deltaCol then Some DiagonalIzquierdaInversa
            else None
        
        match direccionOpt with
        | None -> None
        | Some direccion ->
            // Extraer la palabra seleccionada
            let rec extraerPalabra posActual acc =
                if posActual = fin then
                    let caracter = sopa.Matriz.[posActual.Fila, posActual.Columna]
                    acc + string caracter
                else
                    let caracter = sopa.Matriz.[posActual.Fila, posActual.Columna]
                    extraerPalabra (moverPosicion posActual direccion) (acc + string caracter)
            
            let palabraSeleccionada = extraerPalabra inicio ""
            
            // Verificar si la palabra está en la lista
            sopa.Palabras
            |> List.tryFind (fun p -> p.ToUpper() = palabraSeleccionada.ToUpper())
            |> Option.map (fun palabraEncontrada ->
                { Palabra = palabraEncontrada
                  Inicio = inicio
                  Fin = fin
                  Direccion = direccion
                  EncontradaPorUsuario = true })
    
    /// Marca una palabra como encontrada por el usuario
    let marcarPalabraEncontrada (sopa: SopaLetras) palabraEncontrada =
        { sopa with 
            PalabrasEncontradas = palabraEncontrada :: sopa.PalabrasEncontradas }
    
    /// Obtiene el porcentaje de palabras encontradas por el usuario
    let obtenerProgreso (sopa: SopaLetras) =
        let totalPalabras = List.length sopa.Palabras
        let encontradas = 
            sopa.PalabrasEncontradas 
            |> List.filter (fun p -> p.EncontradaPorUsuario)
            |> List.length
        if totalPalabras = 0 then 0.0
        else float encontradas / float totalPalabras * 100.0
    
    /// Verifica si el juego ha sido completado
    let juegoCompletado (sopa: SopaLetras) =
        let palabrasEncontradasUsuario = 
            sopa.PalabrasEncontradas 
            |> List.filter (fun p -> p.EncontradaPorUsuario)
            |> List.map (fun p -> p.Palabra.ToUpper())
            |> Set.ofList
        
        let todasPalabras = 
            sopa.Palabras 
            |> List.map (fun p -> p.ToUpper())
            |> Set.ofList
        
        palabrasEncontradasUsuario = todasPalabras