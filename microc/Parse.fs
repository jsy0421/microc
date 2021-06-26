(* Lexing and parsing of micro-C programs using fslex and fsyacc *)

module Parse

open System
open System.IO
open System.Text
open FSharp.Text
open Absyn
open Debug

(* Plain parsing from a string, with poor error reporting *)

let fromString (str : string) : program =
    let lexbuf = Lexing.LexBuffer<char>.FromString(str)
    try 
      CPar.Main CLex.Token lexbuf
    with 
      | exn -> let pos = lexbuf.EndPos 
               failwithf "%s near line %d, column %d\n" 
                  (exn.Message) (pos.Line+1) pos.Column
             
(* Parsing from a file *)
let token buf = 
    let res = CLex.Token buf
    info (fun () ->
          match res with
           |CPar.EOF -> printf "%A\n" res
           |_ -> printf "%A, " res
           )
    res

let fromFile (filename : string) =
    use reader = new StreamReader(filename)
    let lexbuf = Lexing.LexBuffer<char>.FromTextReader reader
    try 
      info (fun ()-> printfn "\nToken:")
      let ast = CPar.Main token lexbuf in
        info (fun ()-> printfn "\nAST:");
        ast
    with 
      | exn -> let pos = lexbuf.EndPos 
               failwithf "%s in file %s near line %d, column %d\n" 
                  (exn.Message) filename (pos.Line+1) pos.Column

// let tokens (filename : string) =
//     use reader = new StreamReader(filename)
//     let lexbuf = Lexing.LexBuffer<char>.FromTextReader reader
//     try 
//       let rec loop () = 
//         let tok = CLex.Token lexbuf
//         printf "%A, " tok
//         if tok = CPar.GT then () else loop () 
//       loop ()
//     with 
//       | exn -> let pos = lexbuf.EndPos 
//                failwithf "%s in file %s near line %d, column %d\n" 
//                   (exn.Message) filename (pos.Line+1) pos.Column
//       | :? System.Exception as ex -> ()