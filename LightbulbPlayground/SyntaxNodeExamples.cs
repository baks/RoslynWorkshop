//directives
using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightbulbPlayground
{
    public class SyntaxNodeExamples
    {
        public void Examples()
        {
            //Expressions
            new object();

            //blocks
            try
            {

            } catch(Exception e) { }
            //clauses
            finally { }

            //Statements
            while (true)
            {
            }
        }
    }
}
