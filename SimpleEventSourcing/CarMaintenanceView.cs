using System;

namespace SimpleEventSourcing
{
    /// <summary>
    /// Snapshot in time of the event stream.
    /// </summary>
    public class CarMaintenanceView
    {
        public decimal AverageMaintenanceCost { get; set; }

        public DateTime? OilLastChanged { get; set; }

        public int TotalMainteances { get; set; }
    }
}
