using System;

namespace SimpleEventSourcing
{
    public class OilChanged : CarEvent
    {
        public string OilType { get; set; }
    }

    public class BrakesServiced : CarEvent
    {
    }

    public class BatteryTested : CarEvent
    {
        public double BatteryLevel { get; set; }
    }

    /// <summary>
    /// Represents a single immutable occurance of a car maintenance event
    /// </summary>
    public class CarEvent
    {
        public string ID { get; set; }

        public string AggregateID { get; set; }

        public int Version { get; set; }

        public DateTime Date { get; set; }

        public int Mileage { get; set; }

        public decimal Cost { get; set; }
    }
}
