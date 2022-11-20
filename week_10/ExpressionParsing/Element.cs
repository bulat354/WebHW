using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressionParsing
{
    public abstract class Element
    {
        protected string exp;

        public Element(string exp)
        {
            this.exp = exp;
        }

        public abstract Expression GetExpression(Func<string, object> comparer);
    }

    public class BracketElement : Element
    {
        public bool isOpen;

        public BracketElement(string exp) : base(exp)
        {
            isOpen = exp == "(";
        }

        public override Expression GetExpression(Func<string, object> comparer)
        {
            return null;
        }
    }

    public class OperElement : Element
    {
        public Element left;
        public Element right;

        public OperElement(string exp) : base(exp)
        {
        }

        public override Expression GetExpression(Func<string, object> comparer)
        {
            if (this.left == null)
                return GetExpressionRightOnly(comparer);

            var left = this.left.GetExpression(comparer);
            var right = this.right.GetExpression(comparer);
            switch (exp)
            {
                case "+":
                    return Expression.Add(left, right);
                case "-":
                    return Expression.Subtract(left, right);
                case "*":
                    return Expression.Multiply(left, right);
                case "/":
                    return Expression.Divide(left, right);
                case "%":
                    return Expression.Modulo(left, right);
                case "^":
                    return Expression.Power(left, right);

                case "&":
                    return Expression.And(left, right);
                case "|":
                    return Expression.Or(left, right);

                case "&&":
                    return Expression.AndAlso(left, right);
                case "||":
                    return Expression.OrElse(left, right);

                case "==":
                    return Expression.Equal(left, right);
                case "!=":
                    return Expression.NotEqual(left, right);

                case ">":
                    return Expression.GreaterThan(left, right);
                case "<":
                    return Expression.LessThan(left, right);
                case ">=":
                    return Expression.GreaterThanOrEqual(left, right);
                case "<=":
                    return Expression.LessThanOrEqual(left, right);
            }

            throw new ArgumentException("Unknown operation " + exp);
        }

        public Expression GetExpressionRightOnly(Func<string, object> comparer)
        {
            var right = this.right.GetExpression(comparer);
            switch (exp)
            {
                case "-":
                    return Expression.Negate(right);
                case "!":
                    return Expression.Not(right);
            }

            throw new ArgumentException("Unknown operation " + exp);
        }
    }

    public abstract class NoOperationElement : Element
    {
        protected NoOperationElement(string exp) : base(exp)
        {
        }
    }

    public class ConstantElement : NoOperationElement
    {
        public ConstantElement(string exp) : base(exp)
        {
        }

        public override Expression GetExpression(Func<string, object> comparer)
        {
            if (exp == "true")
                return Expression.Constant(true, typeof(bool));
            if (exp == "false")
                return Expression.Constant(false, typeof(bool));

            if (Regex.IsMatch(exp, @"^[0-9]+$"))
                return Expression.Constant(int.Parse(exp), typeof(int));

            if (Regex.IsMatch(exp, @"^[0-9]+\.[0-9]+$"))
                return Expression.Constant(double.Parse(exp), typeof(double));

            if (Regex.IsMatch(exp, @"^""[^""]*""$"))
                return Expression.Constant(exp, typeof(string));

            if (Regex.IsMatch(exp, @"^'[^']'$"))
                return Expression.Constant(exp[1], typeof(char));
            if (Regex.IsMatch(exp, @"^'\\[^']'$"))
                return Expression.Constant(exp[2], typeof(char));

            throw new ArgumentException("Unknown constant " + exp);
        }
    }

    public class VariableElement : NoOperationElement
    {
        public VariableElement(string exp) : base(exp)
        {
        }

        public override Expression GetExpression(Func<string, object> comparer)
        {
            var obj = comparer.Invoke(exp);
            return Expression.Constant(obj, obj.GetType());
        }
    }

    public class PropertyElement : NoOperationElement
    {
        public Element left;

        public PropertyElement(string exp) : base(exp)
        {
        }

        public override Expression GetExpression(Func<string, object> comparer)
        {
            var obj = left.GetExpression(comparer);
            return Expression.PropertyOrField(obj, exp.TrimStart('.'));
        }
    }

    public class MethodElement : NoOperationElement
    {
        public Element left;
        public Element[] parameters;
        public string name;

        public MethodElement(string exp) : base(exp)
        {
        }

        public override Expression GetExpression(Func<string, object> comparer)
        {
            var obj = left.GetExpression(comparer);
            var parameters = this.parameters
                .Select(x => Expression.Lambda(x.GetExpression(comparer)))
                .ToArray();
            return Expression.Call(obj, name, parameters.Select(x => x.ReturnType).ToArray(), parameters);
        }
    }
}
