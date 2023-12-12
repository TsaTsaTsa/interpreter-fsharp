module Program

open System
open System.Collections.Generic

open Parser
open AST

type Env = Map<string, Expr>
let env: Map<string, Expr> = Map.empty

let rec eval (env: Env) (expr: Expr) : Expr =
    match expr with
    // Following code includes both fully implemented and not implemented in parser functions. 
    // Mathematical ones are working for samples, check README for more information.
    | Const n -> Const n
    | Add (e1, e2) ->
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (n1 + n2)
        | _, _ -> failwith "Invalid addition"
    | Sub (e1, e2) ->
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (n1 - n2)
        | _, _ -> failwith "Invalid substraction"
    | Mul (e1, e2) ->
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (n1 * n2)
        | _, _ -> failwith "Invalid multiplication"
    | Div (e1, e2) ->
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (n1 / n2)
        | _, _ -> failwith "Invalid division"
    | Equal (e1, e2) ->
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (if n1 = n2 then 1 else 0)
        | _, _ -> failwith "Invalid equality check"
    | Greater (e1, e2) ->
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (if n1 > n2 then 1 else 0)
        | _, _ -> failwith "Invalid greater than check"
    | Less (e1, e2) ->  
        match eval env e1, eval env e2 with
        | Const n1, Const n2 -> Const (if n1 < n2 then 1 else 0)
        | _, _ -> failwith "Invalid less than check"
    | Fact e -> 
        let rec factorial n = 
            if n <= 0 then 1 else n * factorial (n - 1)
        match eval env e with
        | Const n -> Const (factorial n)
        | _ -> failwith "Factorial can only be applied to constants"
    | Var x ->
        match Map.tryFind x env with
        | Some v -> eval env v
        | None -> failwith "Undefined variable"
    | Let(x, e1, e2) -> 
        let v1 = eval env e1
        eval (Map.add x v1 env) e2
    | Func(x, body) -> Func(x, body)
    | Apply(f, arg) ->
        match eval env f with
        | Func(x, body) -> eval (Map.add x arg env) body
        | _ -> failwith "Application to non-function"
    | RecFunc(f, x, body) ->
        let rec bindFuncAndBody env = Func(x, eval (Map.add f (bindFuncAndBody env) env) body)
        bindFuncAndBody env
    | Lazy _ -> failwith "Lazy values not implemented"
    | Seq(e1, e2) ->
        let _ = eval env e1
        eval env e2
    | Force expr -> 
        match expr with
        | Lazy exprLazy -> eval env (exprLazy.Force()) 
        | _ -> failwith "Non-lazy expression cannot be forced"
    | Eq(lhs, rhs) ->
        match eval env lhs, eval env rhs with
        | Const l, Const r -> if l = r then Const 1 else Const 0
        | _, _ -> failwith "Equality can only be applied to constants"
    | ReadFile path ->
        try
            Const (int(System.IO.File.ReadAllText path))
        with
        | _ -> failwith "File read error"
    | WriteFile (path, content) ->
        try
            System.IO.File.WriteAllText(path, content)
            Const 0  
        with
        | _ -> failwith "File write error"
    | List exprList ->
        List (List.map (eval env) exprList)
    | Map (f, expr) ->
        match expr with
        | List lst -> List (lst |> List.map (function Const n -> Const (f n) | _ -> failwith "Invalid list mapping"))
        | _ -> failwith "Map operation requires a list"
    | If (cond, thenExpr, elseExpr) ->
        match eval env cond with
        | Const n -> if n <> 0 then eval env thenExpr else eval env elseExpr
        | _ -> failwith "Condition must be a constant"

let printResult (result: Expr) =
    match result with
    | Const n -> printfn "Result: %d" n
    | _       -> printfn "Result is not a constant."

    // Sample code
let input = "(4 + 5 - 6)! - 6/3" 
try
    let ast = parse input
    let result = eval Map.empty ast
    printResult result
with
| ex -> printfn "An error occurred: %s" (ex.Message)