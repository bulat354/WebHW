using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocol.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DPTPFieldAttribute : Attribute
    {
        public byte FieldID { get; }

        public DPTPFieldAttribute(byte fieldId)
        {
            FieldID = fieldId;
        }
    }
}
