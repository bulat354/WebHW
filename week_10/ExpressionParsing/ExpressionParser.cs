using System;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ExpressionParsing
{
    public class LambdaParser
    {
        public Stack<Element> ParseToStack(string expression)
        {
            var regex = new Regex(@"(\+)|(-)|(\*)|(/)|(%)|(\^)|(&)|(\|)|(&&)|(\|\|)|(==)|(!=)|(>=)|(<=)|(>)|(<)|(!)|([0-9]+(\.[0-9]+)?)|([a-zA-Z0-9]+)|(\.[a-zA-Z0-9]+)|(\.[a-zA-Z0-9]+\([^()]\))|(""[^""]*"")|('\\?[^']')|(\()|(\))|(true)|(false)");
            var stackOne = new Stack<Element>();
            var stackTwo = new Stack<Element>();
            foreach (Match match in regex.Matches(expression))
            {
                var str = match.Value;
                switch (str)
                {
                    case "(":
                        stackOne.Push(new BracketElement(str));
                        continue;
                    case ")":
                        while (stackOne.Peek() is not BracketElement)
                            stackTwo.Push(stackOne.Pop());
                        stackOne.Pop();
                        continue;
                }
                if (Regex.IsMatch(str, @"(^\+$)|(^-$)|(^\*$)|(^/$)|(^%$)|(^\^$)|(^&$)|(^\|$)|(^&&$)|(^\|\|$)|(^==$)|(^!=$)|(^>=$)|(^<=$)|(^>$)|(^<$)|(^!$)"))
                    stackOne.Push(new OperElement(str));
                else if (str == "true" || str == "false" || Regex.IsMatch(str, @"(^[0-9]+(\.[0-9]+)?$)|(^""[^""]*""$)|(^'\\?[^']'$)"))
                    stackTwo.Push(new ConstantElement(str));
                else if (str.StartsWith('.'))
                {
                    if (str.EndsWith(')'))
                        stackTwo.Push(new MethodElement(str));
                    else
                        stackTwo.Push(new PropertyElement(str));
                }
                else
                    stackTwo.Push(new VariableElement(str));
            }

            while (stackOne.Count > 0)
                stackTwo.Push(stackOne.Pop());

            return stackTwo;
        }

        public Expression Parse(string expression, Func<string, object> comparer)
        {

        }
    }
}