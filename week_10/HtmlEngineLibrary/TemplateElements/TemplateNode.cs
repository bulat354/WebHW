using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlEngineLibrary.TemplateElements
{
    //{{ <Name> (<Expression>) }}
    public class TemplateNode
    {
        public Match Match { get; }
        public string Content { get; }

        public string Name { get; }
        public string Expression { get; }
        
        public bool IsBegin { get; }
        public bool IsContinuation { get; }
        public bool IsEnd { get; }

        public TemplateNode(Match match)
        {
            Match = match;
            Content = match.Value.Trim('{', '}');

            Name = Regex.Match(Content, @"[a-zA-Z]+").Value;
            Expression = Regex.Match(Content, @"\([^}{]*\)").Value;

            IsBegin = Regex.IsMatch(Content, @"begin\s*$");
            IsContinuation = Regex.IsMatch(Content, @"continue\s*$");
            IsEnd = Regex.IsMatch(Content, @"end\s*$");
        }
    }
}
