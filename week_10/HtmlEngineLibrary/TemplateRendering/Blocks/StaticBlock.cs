namespace HtmlEngineLibrary.TemplateRendering.Blocks
{
    internal class StaticBlock : Block
    {
        private string _content;

        public string GetContent()
        {
            return _content;
        }

        public static StaticBlock Create(string content)
        {
            return new StaticBlock() { _content = content, Type = BlockType.Static };
        }

        public override string ToString()
        {
            return _content;
        }
    }
}
