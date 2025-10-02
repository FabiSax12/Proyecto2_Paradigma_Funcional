module Proyecto2_Lenguajes.Logic.SopaLetras

// ===== TIPOS INMUTABLES =====
type Celda = {
    Fila: int
    Columna: int
    Letra: char
}

type Posicion = {
    Fila: int
    Columna: int
}

type Camino = Celda list

type Direccion =
    | Horizontal
    | Vertical
    | DiagonalDerecha
    | DiagonalIzquierda

type PalabraEncontrada = {
    Palabra: string
    Inicio: Posicion
    Fin: Posicion
    Direccion: Direccion
}

type Estado = {
    Matriz: char[,]
    PalabrasObjetivo: string list
    PalabrasEncontradas: PalabraEncontrada list
}

// ===== FUNCIÓN PARA OBTENER PALABRAS =====

/// Devuelve una lista de palabras predeterminadas en español
let obtenerPalabrasDefault () : string list =
    [
        "CASA"
        "PERRO"
        "GATO"
        "SOL"
        "LUNA"
        "ARBOL"
        "FLOR"
        "LIBRO"
        "AGUA"
        "CIELO"
        "TIERRA"
        "FUEGO"
        "VIENTO"
        "NUBE"
        "RIO"
    ]

/// Devuelve palabras según una dificultad
let obtenerPalabrasPorDificultad (dificultad: string) : string list =
    match dificultad.ToUpper() with
    | "FACIL" ->
        [
            "SOL"
            "MAR"
            "PEZ"
            "OSO"
            "AVE"
        ]
    | "MEDIO" ->
        [
            "CASA"
            "PERRO"
            "GATO"
            "LIBRO"
            "ARBOL"
            "FLOR"
            "LUNA"
        ]
    | "DIFICIL" ->
        [
            "COMPUTADORA"
            "PROGRAMACION"
            "ALGORITMO"
            "FUNCIONAL"
            "INMUTABLE"
            "RECURSION"
            "PARADIGMA"
        ]
    | _ -> obtenerPalabrasDefault()

/// Devuelve palabras de un tema específico
let obtenerPalabrasPorTema (tema: string) : string list =
    match tema.ToUpper() with
    | "ANIMALES" ->
        [
            "PERRO"
            "GATO"
            "LEON"
            "TIGRE"
            "OSO"
            "LOBO"
            "ZORRO"
            "CONEJO"
            "CABALLO"
            "VACA"
        ]
    | "NATURALEZA" ->
        [
            "ARBOL"
            "FLOR"
            "MONTAÑA"
            "RIO"
            "LAGO"
            "BOSQUE"
            "PRADERA"
            "SELVA"
        ]
    | "TECNOLOGIA" ->
        [
            "COMPUTADORA"
            "INTERNET"
            "SOFTWARE"
            "HARDWARE"
            "CODIGO"
            "PROGRAMA"
            "SISTEMA"
        ]
    | "COLORES" ->
        [
            "ROJO"
            "AZUL"
            "VERDE"
            "AMARILLO"
            "NARANJA"
            "MORADO"
            "BLANCO"
            "NEGRO"
        ]
    | _ -> obtenerPalabrasDefault()

// ===== BÚSQUEDA EN PROFUNDIDAD =====

/// Función pura - sin side effects
let vecinos (estado: Estado) (celda: Celda) : (Celda * Direccion) list =
    // Retorna vecinos válidos sin mutar estado
    []

/// Extiende el camino sin side effects
let extender (estado: Estado) (camino: Camino) : Camino list =
    // Extiende el camino sin side effects
    []

/// Búsqueda en profundidad pura
let rec profundidad (estado: Estado) (objetivo: string) : Estado option =
    // Búsqueda en profundidad pura
    None

// ===== GENERACIÓN DE MATRIZ =====

/// Genera una matriz de letras con las palabras insertadas
let GenerarMatriz (palabras: string list) (width: int) (height: int) : char[,] =
    // Por ahora, crear matriz simple con letras aleatorias
    let rnd = System.Random()
    let matriz = Array2D.init height width (fun _ _ ->
        char (int 'A' + rnd.Next(26))
    )

    // TODO: Insertar palabras en la matriz
    // Por ahora devolver matriz con letras aleatorias
    matriz

// ===== VALIDACIÓN Y RESOLUCIÓN =====

/// Valida la selección del usuario
let validarSeleccion
    (estado: Estado)
    (filaInicio: int)
    (colInicio: int)
    (filaFin: int)
    (colFin: int)
    : (bool * string option) =

    // TODO: Implementar validación real
    // Por ahora retornar false
    (false, None)

/// Resuelve automáticamente encontrando todas las palabras
let resolverAutomatico (matriz: char[,]) (palabras: string list) : Estado =
    // TODO: Implementar búsqueda automática usando profundidad
    // Por ahora retornar estado vacío
    {
        Matriz = matriz
        PalabrasObjetivo = palabras
        PalabrasEncontradas = []
    }