module AST 

open System

type Expr =
    | Const of int
    | Add of Expr * Expr
    | Sub of Expr * Expr
    | Mul of Expr * Expr
    | Div of Expr * Expr
    | Equal of Expr * Expr
    | Less of Expr * Expr
    | Greater of Expr * Expr
    | Fact of Expr
    | Var of string
    | Let of string * Expr * Expr
    | Func of string * Expr
    | Apply of Expr * Expr
    | RecFunc of string * string * Expr
    | Seq of Expr * Expr
    | Lazy of Lazy<Expr>
    | Force of Expr
    | Eq of Expr * Expr
    | ReadFile of string
    | WriteFile of string * string
    | List of Expr list
    | Map of (int -> int) * Expr
    | If of Expr * Expr * Expr