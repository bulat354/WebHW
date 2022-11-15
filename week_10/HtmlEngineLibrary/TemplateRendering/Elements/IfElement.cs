

namespace HtmlEngineLibrary.TemplateRendering
{
    internal class IfElement : ContinuedElement
    {
        public IfElement(TemplateNode begin, TemplateNode end, params TemplateNode[] continues)
            : base(begin, end, continues)
        {
        }

        public override string Render(string template, StatementVariables variables)
        {
            var renderer = new TemplateRenderer();
            if (IsTrue(begin.Expression, variables))
                return renderer.Render(
                           GetTemplatePart(template, begin, continues.Length > 0 ? continues[0] : end),
                       variables);

            for (int i = 0; i < continues.Length; i++)
            {
                if (IsTrue(continues[i].Expression, variables))
                    return renderer.Render(
                               GetTemplatePart(template, continues[i], continues.Length > i + 1 ? continues[i + 1] : end),
                           variables);
            }

            return string.Empty;
        }

        private bool IsTrue(string expression, StatementVariables variables)
        {
            if (expression == null || expression.Length == 0)
                return true;

            var result = GetValue(expression, variables);

            if (result is bool boolean)
                return boolean;
            else
                return result != null;
        }
    }
}
