using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestHelper;

namespace RoslynWorkshopAnalyzer.Test.Verifiers
{
    public abstract class CodeRefactoringVerifier : CodeFixVerifier
    {
        protected virtual CodeRefactoringProvider GetCodeRefactoringProvider()
        {
            return null;
        }

        protected void VerifyRefactoring(string oldSource, string newSource, TextSpan textSpan)
        {
            var document = CreateDocument(oldSource, "C#");

            var actions = new List<CodeAction>();
            var context = new CodeRefactoringContext(document, textSpan, (a) => actions.Add(a), CancellationToken.None);
            var codeRefactoringProvider = GetCodeRefactoringProvider();
            codeRefactoringProvider.ComputeRefactoringsAsync(context).Wait();

            if (!actions.Any())
            {
                Assert.Fail();
                return;
            }

            var codeAction = actions.First();
            var operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
            document = solution.GetDocument(document.Id);

            var newDocumentString = GetStringFromDocument(document);

            Assert.AreEqual(newSource, newDocumentString);
        }

        private static string GetStringFromDocument(Document document)
        {
            var simplifiedDoc = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
            var root = simplifiedDoc.GetSyntaxRootAsync().Result;
            root = Formatter.Format(root, Formatter.Annotation, simplifiedDoc.Project.Solution.Workspace);
            return root.GetText().ToString();
        }
    }
}
