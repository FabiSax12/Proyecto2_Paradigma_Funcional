namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System

module ReadFile =
    /// Longitud máxima permitida para las palabras (ajusta según necesites)
    let longitudMaximaPalabra = 15

    /// Lee palabras desde un archivo de texto
    let procesarLineas (lineas: string array) =
        lineas
        |> Array.map (fun linea -> linea.Trim().ToUpper())
        |> Array.filter (fun linea -> not (String.IsNullOrWhiteSpace(linea)))
        |> Array.filter (fun linea -> linea.Length <= longitudMaximaPalabra)
        |> Array.toList

    /// Lee palabras con un límite de longitud personalizado
    let procesarLineasConLimite (longitudMaxima: int) (lineas: string array) =
        lineas
        |> Array.map (fun linea -> linea.Trim().ToUpper())
        |> Array.filter (fun linea -> not (String.IsNullOrWhiteSpace(linea)))
        |> Array.filter (fun linea -> linea.Length <= longitudMaxima)
        |> Array.toList
