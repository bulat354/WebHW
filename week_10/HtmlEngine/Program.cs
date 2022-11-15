using HtmlEngine.Models;
using HtmlEngineLibrary;

namespace HtmlEngine
{
    public class Program
    {
        public static void Main()
        {
            var template = @"
{{ if (Result == ""test1"") begin }} {{Result}}ok
{{ elseif (Result == ""test2"") continue }} {{Result}}ok
{{ else continue }} {{Result}} OK
{{ if end}}
{{foreach (c in Result) begin }} 
Character: {{c}}
{{ foreach end}}";
            Console.WriteLine(new HtmlEngineService().GetHtml(template, new Professor() { Result = "test1" }));
        }
    }
}
