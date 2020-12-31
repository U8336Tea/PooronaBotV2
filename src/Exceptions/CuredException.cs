
namespace PooronaBot.Exceptions
{
    [System.Serializable]
    public class CuredException : System.Exception
    {
        public CuredException() : this("This user took the vaccine like some sort of commie.") { }
        public CuredException(string message) : base(message) { }
        public CuredException(string message, System.Exception inner) : base(message, inner) { }
        protected CuredException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}