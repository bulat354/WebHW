using HtmlEngineLibrary.TemplateRendering.Blocks;
using System.Collections;
using System.Text;

namespace HtmlEngineLibrary.TemplateRendering
{
    public class Template
    {
        private List<Block> _blocks;

        private Template()
        {
            _blocks = new List<Block>();
        }

        public static Template Create(string template)
        {
            var result = new Template();
            result._blocks = GetBlocks(template).ToList();
            return result;
        }

        #region Parsing template
        private static IEnumerable<Block> GetBlocks(string template)
        {
            var str = new StringBuilder();
            for (int i = 0; i < template.Length; i++)
            {
                var symbol = template[i];

                if (symbol == '@')
                {
                    if (str.Length > 0)
                    {
                        yield return Block.CreateStatic(str.ToString());
                        str.Clear();
                    }
                    yield return ParseBlock(template, ref i);
                    i--;
                }
                else
                    str.Append(symbol);
            }
        }

        private static Block ParseBlock(string template, ref int i)
        {
            var label = GetLabel(template, ref i);
            if (label.Length < 1)
                return Block.CreateStatic("@");

            var temp = i;
            string expr = null;
            for (; i < template.Length; i++)
            {
                var symbol = template[i];
                if (symbol == ' ')
                    continue;
                if (symbol == '(')
                {
                    expr = GetExpression(template, ref i);
                    break;
                }
                else
                {
                    i = temp;
                    break;
                }
            }
            return Block.CreateFunc(label, expr);
        }

        private static string GetLabel(string template, ref int i)
        {
            var label = new StringBuilder();
            for (i = i + 1; i < template.Length; i++)
            {
                var symbol = template[i];
                if (symbol <= 'z' && symbol >= 'a')
                    label.Append(symbol);
                else
                    break;
            }

            return label.ToString();
        }

        private static string GetExpression(string template, ref int i)
        {
            var br = 0;
            var expr = new StringBuilder();
            for (i = i + 1; i <= template.Length; i++)
            {
                var symbol = template[i];
                if (symbol == ')' && br == 0)
                {
                    i++; break;
                }
                if (symbol == '(')
                    br++;
                else if (symbol == ')')
                    br--;
                expr.Append(symbol);
            }
            return expr.ToString();
        }
        #endregion

        public string Render(object model)
        {
            var i = -1;
            return string.Join("", RenderUntil(new LocalValues(model), ref i, x => false));
        }

        #region Generating text
        private IEnumerable<string> RenderUntil
            (LocalValues values, ref int cursor, Func<Block, bool> end)
        {
            var result = Enumerable.Empty<string>();
            cursor++;
            for (; cursor < _blocks.Count; cursor++)
            {
                var block = _blocks[cursor];
                if (end(block))
                    break;

                if (block.IsEnd)
                    continue;

                if (block.IsStatic)
                    result = result.Append(RenderStatic(block));
                else if (block.IsPrint)
                    result = result.Append(RenderPrint(block, values));
                else if (block.IsIf || block.IsElseif)
                    result = result.Concat(RenderIf(block, values, ref cursor));
                else if (block.IsElse)
                    result = result.Concat(RenderUntil(values, ref cursor, x => x.IsEnd));
                else if (block.IsForeach)
                    result = result.Concat(RenderForeach(block, values, ref cursor));
            }
            cursor--;
            return result;
        }

        private void SkipUntil(ref int cursor, Func<Block, bool> end)
        {
            for (cursor++; cursor < _blocks.Count; cursor++)
            {
                if (end(_blocks[cursor]))
                    break;
            }
            cursor--;
        }

        private string RenderStatic(Block block)
        {
            return block.ToStatic().GetContent();
        }

        private string RenderPrint(Block block, LocalValues values)
        {
            return block.ToDynamic().GetExpressionResult(values).ToString();
        }

        private IEnumerable<string> RenderIf(Block block, LocalValues values, ref int cursor)
        {
            IEnumerable<string> result;

            var obj = block.ToDynamic().GetExpressionResult(values);

            if (obj is bool boolean)
            {
                if (boolean)
                {
                    result = RenderUntil(values, ref cursor, x => x.IsElse || x.IsElseif || x.IsEnd);
                    SkipUntil(ref cursor, x => x.IsEnd);
                }
                else
                {
                    result = Enumerable.Empty<string>();
                    SkipUntil(ref cursor, x => x.IsEnd || x.IsElse || x.IsElseif);
                }
            }
            else
                throw new ArgumentException($"Expression in {block} must be boolean");

            return result;
        }

        private IEnumerable<string> RenderForeach(Block block, LocalValues values, ref int cursor)
        {
            var split = block.ToDynamic().GetExpression().Split(" in ");
            if (split.Length != 2)
                throw new ArgumentException("Expression is invalid");

            var name = split[0];
            var objects = DynamicBlock.GetExpressionResult(values, split[1]);

            if (objects is not IEnumerable)
                throw new ArgumentException("Expression is invalid");

            var enumerable = objects as IEnumerable;

            var result = Enumerable.Empty<string>();
            int i = cursor;
            foreach (var value in enumerable)
            {
                i = cursor;
                result = result.Concat(RenderUntil(new LocalValues(values, (name, value)), ref i, x => x.IsEnd));
            }
            cursor = i;

            return result;
        }
        #endregion
    }
}
