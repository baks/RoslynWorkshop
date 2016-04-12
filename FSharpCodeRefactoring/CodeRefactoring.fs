namespace FSharpCodeRefactoring

open System.Composition
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax


[<ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "AddBracesToIfRefactoring")>]
[<Shared>]
type AddBracesToIfRefactoring() =
    inherit CodeRefactoringProvider()

    let ifStatementHasBraces (ifStatement:IfStatementSyntax) = 
        match ifStatement.Statement.Kind() with
        | SyntaxKind.Block -> true
        | _ -> false

    let addBraces (ifStatement:IfStatementSyntax) = ifStatement.Statement |> SyntaxFactory.Block

    let createRefactoredDocument (document:Document) (root:SyntaxNode) (ifStatement:IfStatementSyntax) =
        let changedIfStatement = ifStatement |> addBraces |> ifStatement.WithStatement    
        root.ReplaceNode(ifStatement, changedIfStatement) |> document.WithSyntaxRoot |> Task.FromResult

    let registerCodeAction (context:CodeRefactoringContext) (root:SyntaxNode) (ifStatement:IfStatementSyntax) = 
        let codeAction = CodeAction.Create("Add braces", (fun token -> createRefactoredDocument context.Document root ifStatement))
        context.RegisterRefactoring(codeAction)

    override this.ComputeRefactoringsAsync context = 
        async {
            let! root = context.Document.GetSyntaxRootAsync(context.CancellationToken) |> Async.AwaitTask
            let node = root.FindNode(context.Span)
            match node.Kind() with
            | SyntaxKind.IfStatement -> if not(ifStatementHasBraces (node :?> IfStatementSyntax)) then registerCodeAction context root (node :?> IfStatementSyntax)
            | _ -> return ()
        } |> Async.StartAsTask :> Task