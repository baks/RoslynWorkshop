using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynWorkshopAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RoslynWorkshopAnalyzerCodeFixProvider)), Shared]
    public class RoslynWorkshopAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Make constructor protected";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(RoslynWorkshopAnalyzerAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();

            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            var ctorNode = (ConstructorDeclarationSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);

            context.RegisterCodeFix(CodeAction.Create(Title, c => MakeConstructorProtected(context.Document, syntaxRoot, ctorNode, c), Title), diagnostic);
        }

        private Task<Document> MakeConstructorProtected(Document document, SyntaxNode root, ConstructorDeclarationSyntax ctorDeclarationSyntax, CancellationToken cancellationToken)
        {
            //var publicModifierInCtor =
            //    ctorDeclarationSyntax.Modifiers.Single(token => token.IsKind(SyntaxKind.PublicKeyword));

            //var newModifiers = ctorDeclarationSyntax.Modifiers.Replace(publicModifierInCtor,
            //    SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));

            var newDeclaration = ctorDeclarationSyntax.Accept(new PublicCtorToProtectedRewriter());

            var newSyntaxRoot = root.ReplaceNode(ctorDeclarationSyntax, newDeclaration);

            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(newDocument);
        }

        private class PublicCtorToProtectedRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                return
                    node.WithModifiers(
                        SyntaxFactory.TokenList(
                            node.Modifiers.Where(st => !st.IsKind(SyntaxKind.PublicKeyword))
                                .Union(new[] {SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)})));
            }
        }
    }
}