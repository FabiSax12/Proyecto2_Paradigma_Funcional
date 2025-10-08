namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System
open Types
open Auxiliares
open BusquedaProfundidad

module BusquedaPalabras =

    /// Busca una palabra en la matriz usando búsqueda en profundidad
    let buscarPalabra (matriz: char[,]) (palabra: string) =
        let tamaño = Array2D.length1 matriz
        let palabraMayus = palabra.ToUpper()

        // Si la palabra está vacía, no hay nada que buscar
        if String.IsNullOrWhiteSpace(palabra) then
            []
        else
            // Crear estados iniciales para cada posición que coincida con la primera letra
            let primeraLetra = palabraMayus.[0]
            let estadosIniciales =
                seq {
                    for fila in 0 .. tamaño - 1 do
                        for col in 0 .. tamaño - 1 do
                            let pos = { Fila = fila; Columna = col }
                            match obtenerCaracter matriz pos with
                            | Some c when Char.ToUpper(c) = primeraLetra ->
                                yield { Posicion = pos
                                        Camino = [pos]
                                        Palabra = palabraMayus
                                        IndiceActual = 1
                                        Direccion = None }
                            | _ -> ()
                } |> Seq.toList

            // Función objetivo: verifica si se encontró la palabra completa
            let objetivo estado = estado.IndiceActual >= palabraMayus.Length

            // Buscar todas las ocurrencias de la palabra
            let rec buscarTodas estados resultados =
                match estados with
                | [] -> resultados
                | estado :: resto ->
                    match profundidad matriz objetivo [estado] with
                    | Some solucion ->
                        let nuevosEstados = resto |> List.filter (fun e ->
                            e.Posicion <> estado.Posicion || e.Direccion <> None)
                        buscarTodas nuevosEstados (solucion :: resultados)
                    | None ->
                        buscarTodas resto resultados

            let soluciones = buscarTodas estadosIniciales []

            // Convertir soluciones a PalabraEncontrada
            soluciones
            |> List.map (fun sol ->
                let camino = List.rev sol.Camino
                { Palabra = palabra
                  Inicio = List.head camino
                  Fin = List.last camino
                  Direccion = sol.Direccion |> Option.defaultValue Horizontal
                  EncontradaPorUsuario = false })

    /// Busca todas las palabras en la sopa de letras de forma automática
    let encontrarTodasSoluciones (sopa: SopaLetras) (encontradasPorJugador: string list) =
        sopa.Palabras
        |> List.filter (fun palabra -> not (List.contains palabra encontradasPorJugador))
        |> List.collect (fun palabra -> buscarPalabra sopa.Matriz palabra)
