using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BossRush
{
    [System.Serializable]
    public class NoPlayersInRangeException : System.Exception
    {
        public NoPlayersInRangeException() { }
        public NoPlayersInRangeException(string message) : base(message) { }
        public NoPlayersInRangeException(string message, System.Exception inner) : base(message, inner) { }
        protected NoPlayersInRangeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}