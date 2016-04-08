using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynWorkshopAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RoslynWorkshopAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "D001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Design";

        private static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            //context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclarationUsingCSharpSyntaxVisitor, SyntaxKind.ClassDeclaration);
            //context.RegisterSymbolAction(AnalyzeClassSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (!HasAbstractModifier(classDeclaration.Modifiers))
            {
                return;
            }

            foreach (var publicCtor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().Where(IsPublic))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, publicCtor.GetLocation(),
                    classDeclaration.Identifier.ValueText));
            }
        }

        private static void AnalyzeClassDeclarationUsingCSharpSyntaxVisitor(SyntaxNodeAnalysisContext context)
        {
            var className = ((ClassDeclarationSyntax) context.Node).Identifier.ValueText;
            var walker = new AllPublicConstructorsInSyntaxElement();
            walker.Visit(context.Node);

            foreach (var publicCtor in walker.PubliConstructors)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, publicCtor.GetLocation(), className));
            }
        }

        private static void AnalyzeClassSymbol(SymbolAnalysisContext context)
        {
            var namedType = (INamedTypeSymbol) context.Symbol;

            if (!IsNamedTypeSymbolAbstractClass(namedType))
            {
                return;
            }

            foreach (var ctor in namedType.Constructors.Where(IsMethodSymbolPublic))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule,
                    Location.Create(ctor.DeclaringSyntaxReferences.First().SyntaxTree,
                        ctor.DeclaringSyntaxReferences.First().Span),
                    namedType.Name));
            }
        }

        private static bool IsNamedTypeSymbolAbstractClass(INamedTypeSymbol typeSymbol)
            => typeSymbol.TypeKind == TypeKind.Class && typeSymbol.IsAbstract;

        private static bool IsMethodSymbolPublic(IMethodSymbol methodSymbol)
            => methodSymbol.DeclaredAccessibility == Accessibility.Public;

        private static bool IsPublic(ConstructorDeclarationSyntax declaration)
            => declaration.Modifiers.Any(SyntaxKind.PublicKeyword);

        private static bool HasAbstractModifier(SyntaxTokenList modifiers)
            => modifiers.Any(SyntaxKind.AbstractKeyword);

        private class AllPublicConstructorsInSyntaxElement : CSharpSyntaxWalker
        {
            public IEnumerable<ConstructorDeclarationSyntax> PubliConstructors { get; private set; }

            public AllPublicConstructorsInSyntaxElement()
            {
                PubliConstructors = Enumerable.Empty<ConstructorDeclarationSyntax>();
            }   

            public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                if (IsPublic(node))
                {
                    PubliConstructors = PubliConstructors.Union(new[] {node});
                }
            }
        }
    }
}
