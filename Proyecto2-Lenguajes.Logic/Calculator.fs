module Proyecto2_Lenguajes.Logic.Calculator

// Funciones básicas
let add (x: float) (y: float) = x + y
let subtract (x: float) (y: float) = x - y
let multiply x y = x * y
let divide x y =
    if y = 0.0 then None else Some (x / y)

// Función que demuestra composición
let calculateWithTax rate amount =
    amount
    |> multiply (1.0 + rate)
    |> fun result -> System.Math.Round(result, 2)

// Función recursiva
let rec factorial n =
    match n with
    | 0 | 1 -> 1
    | _ -> n * factorial (n - 1)

// Función que trabaja con listas (map, filter, reduce)
let processNumbers numbers =
    numbers
    |> List.filter (fun x -> x > 0)
    |> List.map (fun x -> x * 2)
    |> List.sum