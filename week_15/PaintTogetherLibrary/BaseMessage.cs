using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTogetherLibrary
{
    public abstract class BaseMessage
    {
        public abstract string ToString();
        public abstract BaseMessage FromString(string message);

        public static BaseMessage ParseMessage(string message)
        {
            var split = message.Split(new[] {' '}, 2);
            switch (split[0])
            {
                case "paint":
                    return new PaintPointMessage().FromString(message);
                case "join":
                    return new PlayerJoinMessage().FromString(message);
                case "disconnect":
                    return new PlayerDisconnectMessage().FromString(message);

                default:
                    throw new ArgumentException();
            }
        }
    }
}
