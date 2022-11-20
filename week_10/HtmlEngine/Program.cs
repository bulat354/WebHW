using HtmlEngine.Models;
using HtmlEngineLibrary.TemplateRendering;

namespace HtmlEngine
{
    public class Program
    {
        public static void Main()
        {
            TestIf();
            TestForeach();
            TestValue();
            TestNesting();
        }

        private static void TestIf()
        {
            var text = @"@if (true) yes @end  
@if (false) no @else yes @end 
@if (true) yes @else no @end 
@if (false) no @elseif (true) yes @else no @end
@if (false) no @elseif (false) no @else yes @end";
            Console.WriteLine("----------If Test----------");
            Console.WriteLine(Template.Create(text).Render(null));
        }

        private static void TestForeach()
        {
            var text = @"@foreach (i in results) ok @end";
            Console.WriteLine("----------Foreach Test----------");
            Console.WriteLine(Template.Create(text).Render(new Test1()));
        }

        private static void TestValue()
        {
            var text = @"@print (test2.Test())
@print (number + 104 - (9 * 24) + 203 % 2)
@print (test2.result + "" VERY GOOD"")";
            Console.WriteLine("----------Value Test----------");
            Console.WriteLine(Template.Create(text).Render(new Test1()));
        }

        private static void TestNesting()
        {
            var text = @"@if (true) @print(""OK"") @if (true) @print(""OKx2"") @end @end
@foreach (test in tests) 
@print (test.result) @if (test.result == ""Ok"") @print (""OKx"" + 4) @end 
@end";
            Console.WriteLine("----------Value Test----------");
            Console.WriteLine(Template.Create(text).Render(new Test1()));
        }
    }
}
