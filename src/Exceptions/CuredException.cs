
namespace PooronaBot.Exceptions
{
    [System.Serializable]
    public class CuredException : System.Exception
    {
        public CuredException() : this("The vaccine has rendered this person immune to the virus!") { }
        public CuredException(string message) : base(message) { }
        public CuredException(string message, System.Exception inner) : base(message, inner) { }
        protected CuredException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}