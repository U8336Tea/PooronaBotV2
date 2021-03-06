using System;

namespace PooronaBot.Exceptions
{
    [System.Serializable]
    public class LimitException : Exception
    {
        public LimitException() : this("Stop infecting so many people, you greedy asshole.") { }
        public LimitException(string message) : base(message) { }
        public LimitException(string message, System.Exception inner) : base(message, inner) { }
        protected LimitException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public LimitException(int limit, int count) : this($"Limit is {limit}, but count is {count}.") {}
    }
}