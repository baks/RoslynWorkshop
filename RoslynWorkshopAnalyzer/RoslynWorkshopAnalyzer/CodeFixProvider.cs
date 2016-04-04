using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

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
            await Task.FromResult(0);
        }
    }
}