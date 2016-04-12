using FSharpCodeRefactoring;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using RoslynWorkshopAnalyzer.Test.Verifiers;
using Xunit;

namespace RoslynWorkshopAnalyzer.Test
{
    public class AddBracesRefactoringTests : CodeRefactoringVerifier
    {
        protected override CodeRefactoringProvider GetCodeRefactoringProvider()
        {
            return new AddBracesToIfRefactoring();
        }

        [Fact]
        public void ShouldAddBraces()
        {
            var source = @"
public class Test
{
    public void Method(int x)
    {
        if(x == 10)
            new object();
        return;
    }
}";

            var newSource = @"
public class Test
{
    public void Method(int x)
    {
        if(x == 10)
        {
            new object();
        }

        return;
    }
}";

            VerifyRefactoring(source,newSource, new TextSpan(70, 39));
        }
    }
}
