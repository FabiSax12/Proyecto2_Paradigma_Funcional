namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System
open System.IO

module ReadFile = 
    /// Lee palabras desde un archivo de texto
    let leerPalabrasDesdeArchivo rutaArchivo =
        try
            File.ReadAllLines(rutaArchivo)
            |> Array.map (fun linea -> linea.Trim().ToUpper())
            |> Array.filter (fun linea -> not (String.IsNullOrWhiteSpace(linea)))
            |> Array.toList
        with
        | :? FileNotFoundException ->
            printfn "Archivo no encontrado: %s" rutaArchivo
            []
        | ex ->
            printfn "Error al leer archivo: %s" ex.Message
            []
    