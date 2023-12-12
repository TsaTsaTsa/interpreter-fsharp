[![Open in Visual Studio Code](https://classroom.github.com/assets/open-in-vscode-718a45dd9cf7e7f842a935f5ebbe5719a5e09af4491e668f4dbf3b35d5cca122.svg)](https://classroom.github.com/online_ide?assignment_repo_id=12425455&assignment_repo_type=AssignmentRepo)
# Functional Programming Lab: Write Your Own Compiler/Interpreter

The goal of this lab is to invent your own functional programming language and develop an interpreter/compiler for it.

You may do this lab in the group of 2 or 3 people (or more - but this requires approval from the lecturer):

* 2 people - compiler/interpreter + sample programs + short documentation in README.md (feel free to replace this file with your own documentation)
* 3 people - compiler/interpreter + sample programs + more detailed documentation in GitHub Pages
* 3+ people - in addition to above, this may include some of the following:
  - in-browser IDE
  - Jupyter Notebook support
  - Translation to JavaScript, so that you can execute the program in browser
  - VS Code Extensions

## Task

Your goal would be to invent and implement your own functional programming language. The requirements are:

* It has to closely follow functional programming paradigm, based on either [Lambda calculus](https://en.wikipedia.org/wiki/Lambda_calculus) or [Combinatory Logic](https://en.wikipedia.org/wiki/Combinatory_logic).
* It has to be more or less Turing-complete, i.e. implement recursion
* You should be able to program and run a function to calculate factorial.

> Keep in mind that writing parsers is a tedious task, so try to keep the syntax of the language as simple as possible. 

For inspiration:

* Look into [LISP](https://books.ifmo.ru/file/pdf/1918.pdf) - a programming language with very simple syntax
* Combinatory parsers and [fparsec](https://www.quanttec.com/fparsec/) library, if you want to implement a language with more complex syntax
* [Top-Down Parser in F#](https://github.com/fholm/Vaughan)
* Interesting blog post on [Parsing in F#](https://www.erikschierboom.com/2016/12/10/parsing-text-in-fsharp/)
* Parsing using [FsLex and FsYacc tooling](https://realfiction.net/posts/lexing-and-parsing-in-f/) (not recommended)
* Implementation of [Scheme in F#](https://github.com/AshleyF/FScheme) - you may look at this project for inspiration, but do not borrow code from there!

## Evaluation Criteria

* Turing Completeness
* Program Samples, including, but not limited to factorial
* Originality and beauty of syntax
* Documentation
* Beauty of implementation

Preferred language for implementation is F#.

In the documentation, please mention explicitly which language features you have implemented:

* [ ] Named variables (`let`)
* [ ] Recursion
* [ ] Lazy evaluation
* [ ] Functions
* [ ] Closures
* [ ] Library functions: File IO
* [ ] Lists / Sequences
* [ ] Library functions: Lists/Sequences

## Repository

You need to work on the code in GitHub Classroom repo. After you finish the task, it is **highly recommended** to fork this code in your own GitHub accounts, so that is can act as your portfolio.

## Working Step-By-Step

Because the projects is rather big, you need to do it in several stages, checking in your code to GitHub in between:

* Stage 1: Developing abstract syntax tree and parser for your language + one sample program
* Stage 2: Developing interpreter/compiler for your language
* Stage 3: Writing sample programs and documentation

> Of course, you are welcome to modify the language at later stages as you see fit.

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

