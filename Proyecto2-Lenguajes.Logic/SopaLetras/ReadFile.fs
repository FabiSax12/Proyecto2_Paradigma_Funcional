namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System

module ReadFile =
    /// Lee palabras desde un archivo de texto
    let procesarLineas (lineas: string array) =
        lineas
        |> Array.map (fun linea -> linea.Trim().ToUpper())
        |> Array.filter (fun linea -> not (String.IsNullOrWhiteSpace(linea)))
        |> Array.toList
