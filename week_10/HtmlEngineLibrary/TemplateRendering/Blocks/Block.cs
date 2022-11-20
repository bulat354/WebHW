namespace HtmlEngineLibrary.TemplateRendering.Blocks
{
    internal abstract class Block
    {
        public BlockType Type { get; protected set; }

        public bool IsEnd { get { return Type == BlockType.End; } }
        public bool IsIf { get { return Type == BlockType.If; } }
        public bool IsElseif { get { return Type == BlockType.ElseIf; } }
        public bool IsElse { get { return Type == BlockType.Else; } }
        public bool IsForeach { get { return Type == BlockType.Foreach; } }
        public bool IsStatic { get { return Type == BlockType.Static; } }
        public bool IsPrint { get { return Type == BlockType.Print; } }

        public StaticBlock ToStatic() => this as StaticBlock;
        public DynamicBlock ToDynamic() => this as DynamicBlock;

        public static Block CreateFunc(string label, string expr)
        {
            if (expr == null || expr.Length < 0)
                return LabelBlock.Create(label);

            return DynamicBlock.Create(label, expr);
        }

        public static Block CreateStatic(string str)
        {
            return StaticBlock.Create(str);
        }

        public enum BlockType
        {
            None,
            End,
            If, ElseIf, Else, Foreach,
            Print, Static
        }
    }
}
