namespace SprintQuiz.Api.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public bool EntityExists { get; }

        public ConcurrencyException(string message, bool entityExists = true)
            : base(message)
        {
            EntityExists = entityExists;
        }

        public ConcurrencyException(string message, Exception innerException, bool entityExists = true)
            : base(message, innerException)
        {
            EntityExists = entityExists;
        }
    }
}