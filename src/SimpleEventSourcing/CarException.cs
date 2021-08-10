using System;

namespace SimpleEventSourcing
{
    /// <summary>
    /// Represents violations of the business logic.
    /// </summary>
    public class CarException : Exception
    {
        public CarException(string message) : base(message)
        {
        }
    }
}
