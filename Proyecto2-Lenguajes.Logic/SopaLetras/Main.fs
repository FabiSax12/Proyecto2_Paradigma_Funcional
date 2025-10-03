namespace Proyecto2_Lenguajes.Logic.SopaLetras

open System

open Types

module SopaLetras =    
    // ============================================================================
    // FUNCIONES AUXILIARES BÁSICAS
    // ============================================================================
    
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
    
    // ============================================================================
    // BÚSQUEDA EN PROFUNDIDAD - FUNCIONES PRINCIPALES
    // ============================================================================
    
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
            // Si encontramos la palabra completa, devolvemos el camino
            if estado.IndiceActual >= estado.Palabra.Length then
                Some estado
            // Si el objetivo es una función que evalúa el estado
            elif objetivo estado then
                Some estado
            else
                // Extender el estado actual y continuar la búsqueda
                let nuevos = extender matriz [estado]
                profundidad matriz objetivo (nuevos @ resto)
    
    // ============================================================================
    // BÚSQUEDA DE PALABRAS
    // ============================================================================
    
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
    let encontrarTodasSoluciones (sopa: SopaLetras) =
        sopa.Palabras
        |> List.collect (fun palabra -> buscarPalabra sopa.Matriz palabra)
    
    // ============================================================================
    // GENERACIÓN DE SOPA DE LETRAS
    // ============================================================================
    
    /// Calcula el tamaño óptimo de la matriz basándose en las palabras
    let calcularTamañoMatriz palabras =
        let longitudMaxima = 
            palabras 
            |> List.map String.length 
            |> List.fold max 0
        
        let cantidadPalabras = List.length palabras
        let tamañoMinimo = max longitudMaxima 10
        let tamañoOptimo = max tamañoMinimo (int (sqrt (float cantidadPalabras) * 5.0))
        min tamañoOptimo 30 // Límite superior de 30x30
    
    /// Intenta colocar una palabra en una posición y dirección específicas
    let intentarColocarPalabra (matriz: char[,]) (palabra: string) pos direccion =
        let tamaño = Array2D.length1 matriz
        let palabraMayus = palabra.ToUpper()
        let longitudPalabra = palabraMayus.Length
        
        // Verificar si la palabra cabe en esta posición y dirección
        let rec verificarEspacio i posActual =
            if i >= longitudPalabra then
                true
            elif not (posicionValida tamaño posActual) then
                false
            else
                let caracterActual = matriz.[posActual.Fila, posActual.Columna]
                let caracterPalabra = palabraMayus.[i]
                if caracterActual = ' ' || caracterActual = caracterPalabra then
                    let siguientePos = moverPosicion posActual direccion
                    verificarEspacio (i + 1) siguientePos
                else
                    false
        
        if verificarEspacio 0 pos then
            // Colocar la palabra
            let nuevaMatriz = Array2D.copy matriz
            let rec colocar i posActual =
                if i < longitudPalabra then
                    nuevaMatriz.[posActual.Fila, posActual.Columna] <- palabraMayus.[i]
                    colocar (i + 1) (moverPosicion posActual direccion)
            colocar 0 pos
            Some nuevaMatriz
        else
            None
    
    /// Genera una matriz de sopa de letras con las palabras dadas
    let generarSopaLetras (palabras: string list) (seed: int) =
        let random = Random(seed)
        let palabrasFiltradas = 
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