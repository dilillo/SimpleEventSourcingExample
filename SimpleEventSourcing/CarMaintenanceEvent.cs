using System;

namespace SimpleEventSourcing
{
    /// <summary>
    /// Represents a single immutable occurance of a car maintenance event
    /// </summary>
    public class CarMaintenanceEvent
    {
        public CarMaintenanceEvent()
        {
        }

        public CarMaintenanceEvent(string aggregateID, int version, CarMaintenceTypes type, DateTime date, int mileage, decimal cost)
        {
            AggregateID = aggregateID;
            CarMaintenceType = type;
            Cost = cost;
            Date = date;
            ID = Guid.NewGuid().ToString();
            Mileage = mileage;
            Version = version;
        }

        public string ID { get; set; }

        public string AggregateID { get; set; }

        public int Version { get; set; }

        public DateTime Date { get; set; }

        public int Mileage { get; set; }

        public decimal Cost { get; set; }

        public CarMaintenceTypes CarMaintenceType { get; set; }
    }
}
