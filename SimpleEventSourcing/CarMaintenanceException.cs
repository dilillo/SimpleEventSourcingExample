using System;

namespace SimpleEventSourcing
{
    /// <summary>
    /// Represents violations of the business logic.
    /// </summary>
    public class CarMaintenanceException : Exception
    {
        public CarMaintenanceException(string message) : base(message)
        {
        }
    }
}
