

namespace HtmlEngineLibrary.TemplateRendering
{
    //{{ <name> (<expression>) [begin|continue|end] }}
    internal interface ITemplateElement
    {
        string Render(string template, StatementVariables variables);
        int GetEndIndex();
        int GetStartIndex();
    }
}
