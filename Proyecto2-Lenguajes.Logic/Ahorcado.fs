module Proyecto2_Lenguajes.Logic.Ahorcado

open System

type EstadoJuego = {
    PalabraSecreta: string
    LetrasAdivinadas: Set<char>
    IntentosRestantes: int
}

type DatosVictoria = { Tiempo: float } 

type Resultado =
    | Victoria of DatosVictoria
    | Derrota  
    | EnJuego  

/// <summary>
/// Inicia un nuevo juego seleccionando una palabra aleatoria de la lista.
/// </summary>
let iniciarJuego (listaPalabras: string list) (maxIntentos: int) =
    let random = Random()
    let palabra = listaPalabras.[random.Next(listaPalabras.Length)].ToUpper()
    {
        PalabraSecreta = palabra
        LetrasAdivinadas = Set.empty
        IntentosRestantes = maxIntentos
    }

/// <summary>
/// Procesa el intento de adivinar una letra y devuelve un nuevo estado del juego.
/// </summary>
let intentarLetra (letra: char) (estado: EstadoJuego) =
    let letraNormalizada = Char.ToUpper(letra)
    let yaAdivinada = estado.LetrasAdivinadas.Contains(letraNormalizada)
    let letraCorrecta = estado.PalabraSecreta.Contains(letraNormalizada)

    if yaAdivinada then
        estado // No hay cambios si la letra ya fue intentada
    else
        let nuevoSetLetras = estado.LetrasAdivinadas.Add(letraNormalizada)
        let nuevosIntentos =
            if letraCorrecta then estado.IntentosRestantes
            else estado.IntentosRestantes - 1
        
        { estado with
            LetrasAdivinadas = nuevoSetLetras
            IntentosRestantes = nuevosIntentos
        }

/// <summary>
/// Devuelve la representación visible de la palabra secreta (ej: "C _ S A").
/// </summary>
let palabraVisible (estado: EstadoJuego) =
    estado.PalabraSecreta
    |> Seq.map (fun c -> if estado.LetrasAdivinadas.Contains(c) then c else '_')
    |> String.Concat
    |> fun s -> String.Join(" ", s.ToCharArray())

/// <summary>
/// Verifica el estado actual del juego para determinar si hay victoria, derrota o si continúa.
/// </summary>
let verificarEstado (estado: EstadoJuego) (tiempoTranscurrido: float) =
    let palabraAdivinada =
        estado.PalabraSecreta
        |> Seq.forall (fun c -> estado.LetrasAdivinadas.Contains(c))

    if palabraAdivinada then
        let datosVictoria = { Tiempo = tiempoTranscurrido }
        Victoria datosVictoria
    elif estado.IntentosRestantes <= 0 then
        Derrota
    else
        EnJuego
