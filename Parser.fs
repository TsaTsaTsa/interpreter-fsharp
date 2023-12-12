module Parser

open System
open System.Collections.Generic
open System.Text.RegularExpressions

open AST

type Token =
    | TInt of int
    | TPlus
    | TMinus
    | TMul
    | TDiv
    | TEqual
    | TGreater
    | TLess
    | TFact 
    | TLParen
    | TRParen
    | TVar of string
    | TEOF

let tokenize (input: string) =
    let regex = Regex(@"(\d+)|(\+)|(\-)|(\*)|(\/)|(\=)|(\>)|(\<)|(!)|(\()|(\))|([a-zA-Z][a-zA-Z0-9]*)", RegexOptions.Compiled)
    let matches = regex.Matches(input)
    [ for m in matches do
        let value = m.Value
        match value with
        | "+" -> yield TPlus
        | "-" -> yield TMinus
        | "*" -> yield TMul
        | "/" -> yield TDiv
        | "=" -> yield TEqual
        | ">" -> yield TGreater
        | "<" -> yield TLess
        | "!" -> yield TFact
        | "(" -> yield TLParen
        | ")" -> yield TRParen
        | _ when Regex.IsMatch(value, @"^\d+$") -> yield TInt (Int32.Parse(value))
        | _ when Regex.IsMatch(value, @"^[a-zA-Z][a-zA-Z0-9]*$") -> yield TVar(value)
        | _ -> failwith "Unknown token"
    ] @ [TEOF]

// parseFactor deals with individual numbers and variables, factorial operation as well as parentheses
let rec parseFactor (tokens: Token list) =
    match tokens with
    | TInt n :: TFact :: rest -> (Fact (Const n), rest)
    | TVar v :: TFact :: rest -> (Fact (Var v), rest)
    | TLParen :: rest ->
        let (exp, afterExp) = parseExpr rest
        match afterExp with
        | TRParen :: TFact :: rest' -> (Fact exp, rest')
        | TRParen :: rest' -> (exp, rest')
        | _ -> failwith "Expected closing parenthesis"
    | TInt n :: rest -> (Const n, rest)
    | TVar v :: rest -> (Var v, rest)
    | _ -> failwith "Invalid factor syntax"
// parseTerm and parseTermRest deal with multiplication and division
and parseTerm (tokens: Token list) =
    let (fact, rest) = parseFactor tokens
    parseTermRest fact rest
and parseTermRest (left: Expr) (tokens: Token list) =
    match tokens with
    | TMul :: rest ->
        let (right, rest') = parseFactor rest
        parseTermRest (Mul (left, right)) rest'
    | TDiv :: rest ->
        let (right, rest') = parseFactor rest
        parseTermRest (Div (left, right)) rest'
    | _ -> (left, tokens)
// parseExpr and parseExprRest deal with addition, substraction, equality, comparisons
and parseExpr (tokens: Token list) =
    let (term, rest) = parseTerm tokens
    parseExprRest term rest
and parseExprRest (left: Expr) (tokens: Token list) =
    match tokens with
    | TPlus :: rest ->
        let (right, rest') = parseTerm rest
        parseExprRest (Add (left, right)) rest'
    | TMinus :: rest ->
        let (right, rest') = parseTerm rest
        parseExprRest (Sub (left, right)) rest'
    | TEqual :: rest ->
        let (right, rest') = parseExpr rest
        (Equal (left, right), rest')
    | TGreater :: rest ->
        let (right, rest') = parseExpr rest
        (Greater (left, right), rest')
    | TLess :: rest ->
        let (right, rest') = parseExpr rest
        (Less (left, right), rest')
    | _ -> (left, tokens)
    
let parse (input: string) =
    let tokens = tokenize input
    fst (parseExpr tokens)