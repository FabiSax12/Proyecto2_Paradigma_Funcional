namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System

open Types
open Auxiliares

module BusquedaProfundidad =
    /// Función vecinos: Genera los estados vecinos válidos a partir del estado actual
    /// Esta función implementa la estrategia de búsqueda vista en clase
    let vecinos (matriz: char[,]) estado =
        let tamaño = Array2D.length1 matriz

        // Si ya completamos la palabra, no hay vecinos
        if estado.IndiceActual >= estado.Palabra.Length then
            []
        else
            let caracterBuscado = estado.Palabra.[estado.IndiceActual]

            // Si aún no tenemos dirección, exploramos todas las direcciones
            match estado.Direccion with
            | None ->
                // Generar vecinos en todas las direcciones posibles
                todasDirecciones
                |> List.map (fun dir ->
                    let nuevaPos = moverPosicion estado.Posicion dir
                    (dir, nuevaPos))
                |> List.filter (fun (_, pos) -> posicionValida tamaño pos)
                |> List.filter (fun (_, pos) ->
                    match obtenerCaracter matriz pos with
                    | Some c -> Char.ToUpper(c) = Char.ToUpper(caracterBuscado)
                    | None -> false)
                |> List.map (fun (dir, pos) ->
                    { Posicion = pos
                      Camino = pos :: estado.Camino
                      Palabra = estado.Palabra
                      IndiceActual = estado.IndiceActual + 1
                      Direccion = Some dir })
            | Some dir ->
                // Ya tenemos una dirección, seguimos en esa dirección
                let nuevaPos = moverPosicion estado.Posicion dir
                if posicionValida tamaño nuevaPos then
                    match obtenerCaracter matriz nuevaPos with
                    | Some c when Char.ToUpper(c) = Char.ToUpper(caracterBuscado) ->
                        [{ Posicion = nuevaPos
                           Camino = nuevaPos :: estado.Camino
                           Palabra = estado.Palabra
                           IndiceActual = estado.IndiceActual + 1
                           Direccion = Some dir }]
                    | _ -> []
                else
                    []

    /// Función extender: Extiende un camino con sus vecinos
    /// Usa funciones de orden superior como se vio en clase
    let extender (matriz: char[,]) camino =
        camino |> List.collect (vecinos matriz)

    /// Función profundidad: Implementa búsqueda en profundidad recursiva
    /// Sigue la misma lógica vista en clase
    let rec profundidad (matriz: char[,]) objetivo caminos =
        match caminos with
        | [] -> None
        | estado :: resto ->
            if objetivo estado then
                Some estado
            else
                let nuevos = extender matriz [estado]
                profundidad matriz objetivo (nuevos @ resto)