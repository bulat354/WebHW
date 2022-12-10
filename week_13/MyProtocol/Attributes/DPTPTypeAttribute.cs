using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocol.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DPTPTypeAttribute : Attribute
    {
        public byte Type { get; }
        public byte SubType { get; }

        public DPTPTypeAttribute(byte type, byte subType)
        {
            Type = type;
            SubType = subType;
        }
    }
}
