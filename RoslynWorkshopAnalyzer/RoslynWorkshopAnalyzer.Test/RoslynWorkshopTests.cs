using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using TestHelper;
using Xunit;

namespace RoslynWorkshopAnalyzer.Test
{
    public class RoslynWorkshopTests : CodeFixVerifier
    {
        [Fact]
        public void DoesNotTriggerDiagnostic()
        {
            var source = @"
public abstract class SomeAbstractClass
{
    protected SomeAbstractClass()
    {
    }
}";

            VerifyCSharpDiagnostic(source, new DiagnosticResult[0]);
        }

        [Fact]
        public void DoesTriggerDiagnostic()
        {
            var source = @"
public abstract class SomeAbstractClass
{
    public SomeAbstractClass()
    {
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = RoslynWorkshopAnalyzerAnalyzer.DiagnosticId,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 4, 5)},
                Message = string.Format(Resources.AnalyzerMessageFormat, "SomeAbstractClass"),
                Severity = DiagnosticSeverity.Warning
            };

            VerifyCSharpDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public void DoesTriggerDiagnosticForAllPublicCtors()
        {
            var source = @"
public abstract class SomeAbstractClass
{
    public SomeAbstractClass()
    {
    }

    public SomeAbstractClass(int x)
    {
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = RoslynWorkshopAnalyzerAnalyzer.DiagnosticId,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 5) },
                Message = string.Format(Resources.AnalyzerMessageFormat, "SomeAbstractClass"),
                Severity = DiagnosticSeverity.Warning
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = RoslynWorkshopAnalyzerAnalyzer.DiagnosticId,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 5) },
                Message = string.Format(Resources.AnalyzerMessageFormat, "SomeAbstractClass"),
                Severity = DiagnosticSeverity.Warning
            };

            VerifyCSharpDiagnostic(source, expectedDiagnostic, expectedDiagnostic2);
        }

        [Fact]
        public void DoesFixPublicCtorToProtected()
        {
            var source = @"
public abstract class SomeAbstractClass
{
    public SomeAbstractClass()
    {
    }
}";

            var expected = @"
public abstract class SomeAbstractClass
{
    protected SomeAbstractClass()
    {
    }
}";

            VerifyCSharpFix(source, expected);
        }

        [Fact]
        public void DoesFixManyCtorsToProtected()
        {
            var source = @"
public abstract class SomeAbstractClass
{
    public SomeAbstractClass()
    {
    }

    public SomeAbstractClass(int x)
    {
    }
}";

            var expected = @"
public abstract class SomeAbstractClass
{
    protected SomeAbstractClass()
    {
    }

    protected SomeAbstractClass(int x)
    {
    }
}";

            VerifyCSharpFix(source, expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RoslynWorkshopAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new RoslynWorkshopAnalyzerAnalyzer();
        }
    }
}