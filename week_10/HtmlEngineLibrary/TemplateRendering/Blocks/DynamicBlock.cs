using NReco.Linq;

namespace HtmlEngineLibrary.TemplateRendering.Blocks
{
    internal class DynamicBlock : Block
    {
        protected string _expression;

        public object? GetExpressionResult(LocalValues values)
        {
            return GetExpressionResult(values, _expression);
        }

        public static object? GetExpressionResult(LocalValues values, string expression)
        {
            return new LambdaParser().Eval(expression, values.GetValue);
        }

        public string GetExpression() { return _expression; }

        public static DynamicBlock Create(string label, string expr)
        {
            if (expr == null || expr.Length == 0 || label == null || label.Length == 0)
                throw new ArgumentException("Invalid block '@{name} ({expr})'");

            DynamicBlock result = new DynamicBlock() { _expression = expr };
            switch (label)
            {
                case "if":
                    result.Type = BlockType.If;
                    break;
                case "elseif":
                    result.Type = BlockType.ElseIf;
                    break;
                case "foreach":
                    result.Type = BlockType.Foreach;
                    break;
                case "print":
                    result.Type = BlockType.Print;
                    break;

                default:
                    throw new ArgumentException($"Unknown block '@{label} ({expr})'");
            }

            return result;
        }

        public override string ToString()
        {
            return $"@{Type.ToString().ToLower()} ({_expression})";
        }
    }
}
