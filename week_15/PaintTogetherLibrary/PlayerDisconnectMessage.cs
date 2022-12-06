using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherLibrary
{
    public class PlayerDisconnectMessage : BaseMessage
    {
        public string Name { get; set; }

        public override BaseMessage FromString(string message)
        {
            var split = message.Split(new[] { ' ' }, 2);
            if (split.Length != 2 || split[0] != "disconnect")
                throw new ArgumentException();

            Name = split[1];
            return this;
        }

        public override string ToString()
        {
            return $"disconnect {Name}";
        }
    }
}
