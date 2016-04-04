using System;
using System.Linq;
using System.Text;
using System.IO;

namespace LightbulbPlayground
{
    public class CodeRefactoringDemo
    {
        public void ProcessSomething()
        {
            var numbers = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            var sum = numbers.Sum();

            Console.WriteLine(sum);
        }
    }
}
