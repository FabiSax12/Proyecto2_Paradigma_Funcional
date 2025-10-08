namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System

open Types
open Auxiliares

module Generador =

    /// Calcula el tamaño óptimo de la matriz basándose en las palabras
    let calcularTamañoMatriz palabras =
        let longitudMaxima =
            palabras
            |> List.map String.length
            |> List.fold max 0

        let cantidadPalabras = List.length palabras
        let tamañoMinimo = max longitudMaxima 5
        let tamañoOptimo = max tamañoMinimo (int (sqrt (float cantidadPalabras) * 5.0))
        min tamañoOptimo 20 // Límite superior de 30x30

    /// Intenta colocar una palabra en una posición y dirección específicas
    let intentarColocarPalabra (matriz: char[,]) (palabra: string) pos direccion =
        let tamaño = Array2D.length1 matriz
        let palabraMayus = palabra.ToUpper()
        let longitudPalabra = palabraMayus.Length

        let rec verificarEspacio i posActual =
            if i >= longitudPalabra then true
            elif not (posicionValida tamaño posActual) then false
            else
                let caracterActual = matriz.[posActual.Fila, posActual.Columna]
                let caracterPalabra = palabraMayus.[i]
                if caracterActual = ' ' || caracterActual = caracterPalabra then
                    verificarEspacio (i + 1) (moverPosicion posActual direccion)
                else false

        if verificarEspacio 0 pos then
            // ✅ Generar lista de posiciones y caracteres, luego crear matriz nueva
            let rec generarCambios i posActual acc =
                if i >= longitudPalabra then acc
                else
                    let cambio = (posActual.Fila, posActual.Columna, palabraMayus.[i])
                    generarCambios (i + 1) (moverPosicion posActual direccion) (cambio :: acc)

            let cambios = generarCambios 0 pos []

            // ✅ Crear nueva matriz aplicando todos los cambios de forma funcional
            let nuevaMatriz = Array2D.init (Array2D.length1 matriz) (Array2D.length2 matriz) (fun i j ->
                match List.tryFind (fun (f, c, _) -> f = i && c = j) cambios with
                | Some (_, _, caracter) -> caracter
                | None -> matriz.[i, j])

            Some nuevaMatriz
        else
            None

    /// Genera una matriz de sopa de letras con las palabras dadas
    let generarSopaLetras (palabras: string list) (seed: int) =
        let random: Random = Random(seed)
        let palabrasFiltradas: string list =
            palabras
            |> List.filter (fun p -> not (String.IsNullOrWhiteSpace(p)))
            |> List.distinct

        let tamaño = calcularTamañoMatriz palabrasFiltradas
        let matriz = Array2D.create tamaño tamaño ' '

        // Intentar colocar cada palabra
        let rec colocarPalabras palabrasRestantes matrizActual =
            match palabrasRestantes with
            | [] -> matrizActual
            | palabra :: resto ->
                let intentos = 100
                let rec intentar n =
                    if n <= 0 then
                        matrizActual // No se pudo colocar, continuar con la siguiente
                    else
                        let fila = random.Next(tamaño)
                        let col = random.Next(tamaño)
                        let pos = { Fila = fila; Columna = col }
                        let direccion = todasDirecciones.[random.Next(todasDirecciones.Length)]

                        match intentarColocarPalabra matrizActual palabra pos direccion with
                        | Some nuevaMatriz -> nuevaMatriz
                        | None -> intentar (n - 1)

                let matrizConPalabra = intentar intentos
                colocarPalabras resto matrizConPalabra

        let matrizConPalabras = colocarPalabras palabrasFiltradas matriz

        // Rellenar espacios vacíos con letras aleatorias
        let letras = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ"
        let matrizCompleta = Array2D.init tamaño tamaño (fun i j ->
            if matrizConPalabras.[i, j] = ' ' then
                letras.[random.Next(letras.Length)]
            else
                matrizConPalabras.[i, j])

        { Matriz = matrizCompleta
          Tamaño = tamaño
          Palabras = palabrasFiltradas
          PalabrasEncontradas = [] }
