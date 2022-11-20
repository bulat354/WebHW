using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEngineLibrary.TemplateRendering.Blocks
{
    internal class LabelBlock : Block
    {
        public static LabelBlock Create(string label)
        {
            if (label == null || label.Length < 0) throw new ArgumentNullException();

            switch (label)
            {
                case "end":
                    return new LabelBlock() { Type = BlockType.End };
                case "else":
                    return new LabelBlock() { Type = BlockType.Else }; 
            }

            throw new ArgumentException($"Unknown block lable '{label}'");
        }
    }
}
