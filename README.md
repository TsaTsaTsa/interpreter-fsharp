# Документация 

## AST

Язык поддерживает основные арифметические операции, обработку целочисленных значений, а также функцию факториала, логические сравнения и скобки.

Не реализовано на уровне парсинга, но обрабатывается: переменные, определения функций, условные операторы, операции с файлами, списки и функции отображения.

### Типы данных
- `Expr`: Представляет узлы абстрактного синтаксического дерева.
  - `Const of int`: Константное целочисленное значение.
  - `Add`, `Sub`, `Mul`, `Div`: Бинарные арифметические операции.
  - `Equal`, `Less`, `Greater`: Бинарные логические сравнения.
  - `Fact`: Факториальная операция.
  - `Var of string`: Переменная.
  - `Let of string * Expr * Expr`: Присваивание переменной.
  - `Func of string * Expr`: Определение функции.
  - `Apply of Expr * Expr`: Применение функции.
  - `RecFunc of string * string * Expr`: Рекурсивное определение функции.
  - `Seq of Expr * Expr`: Последовательность выражений.
  - `Lazy of Lazy<Expr>`: Ленивое вычисление.
  - `Force of Expr`: Принудительное вычисление ленивого выражения.
  - `Eq of Expr * Expr`: Проверка равенства.
  - `ReadFile of string`: Чтение целочисленного содержимого из файла.
  - `WriteFile of string * string`: Запись строкового содержимого в файл.
  - `List of Expr list`: Список выражений.
  - `Map of (int -> int) * Expr`: Применение функции к списку.
  - `If of Expr * Expr * Expr`: Условное выражение.

## Parser

Описана функция для токенизации и парсинга входных строк в абстрактные синтаксические деревья.

### Типы данных
- `Token`: Представляет отдельные токены во входной строке.
  - `TInt of int`: Целочисленная константа.
  - `TPlus`, `TMinus`, `TMul`, `TDiv`: Арифметические операторы.
  - `TEqual`, `TGreater`, `TLess`, `TFact`: Операторы сравнения и факториал.
  - `TLParen`, `TRParen`: Левая и правая круглые скобки.
  - `TVar of string`: Переменная.
  - `TEOF`: Маркер конца файла.

### Функции
- `tokenize (input: string)`: Преобразует входную строку в список токенов.
- `parseFactor (tokens: Token list)`: Парсит целочисленные константы, переменные, скобки, а также факториал из списка токенов.
- `parseExpr (tokens: Token list)`: Основная часть функции, которая парсит сложения и вычитания из списка токенов.
- `parseExprRest (left: Expr) (tokens: Token list)`: Дополнительная отделённая часть parseExpr.
- `parseTerm (tokens: Token list)`: Основная часть функции, которая парсит умножения и деления из списка токенов.
- `parseTermRest (left: Expr) (tokens: Token list)`: Дополнительная отделённая часть parseTerm.
- `parse (input: string)`: Парсит входную строку в абстрактное синтаксическое дерево.

## Program

Среда и функции оценки AST.

### Типы данных
- `Env`: Окружение, отображающее имена переменных на выражения.

### Функции
- `eval (env: Env) (expr: Expr): Expr`: Оценивает выражение в заданном окружении.
- `printResult (result: Expr)`: Выводит результат вычисления выражения.

## Пример использования

```fsharp
let input = "(3 + 5 - 1)! - 3 / 3"
try
    let ast = parse input
    let result = eval Map.empty ast
    printResult result
with
| ex -> printfn "An error occurred: %s" (ex.Message)
```
Данный пример кода токенизирует, парсит, оценивает и выводит результат заданного выражения.

```fsharp
// Expression with variables
let input = Add(Var "x", Const 5)
```

```fsharp
// Simple function
let input = Func("x", Add(Var "x", Const 5))
```

## Credits

Do not forget to mention your team in the README.md file, specifying also who did what. Also, README.md should include a short tutorial on your language, and some short code samples.

Name | Role in the project
------------------|---------------------
Родионова Юлиана Сергеевна | Разработка изначальной версии парсера и основной программы-интерпретатора, внесение конечных правок и добавление комментариев к коду; 
Цанцариди Елена Юрьевна | Создание AST, доработка интерпретатора и примеров, написание документации;
Мыскин Николай Андреевич | Работа над парсером, включая функции парсинга и токенизации, помощь в работе над доработкой интерпретатора;

<img src="https://soshnikov.com/images/byhuman_en.png" height="25px"/>

> If you use Generative AI when writing this code (ChatGPT, GitHub Copilot and such), you need to mention it here (and remove the banner above), and briefly describe how it was used, and how it made you more productive. *Using Generative AI without explicitly mentioning it is a violation of academic conduct!* 

