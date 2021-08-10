using System;

namespace SimpleEventSourcing
{
    public class CarEventFactory
    {
        public static T Create<T>(string aggregateID, int version, DateTime date, int mileage, decimal cost, Action<T> configurator = null) where T : CarEvent, new()
        {
            var @event = new T
            {
                AggregateID = aggregateID,
                Cost = cost,
                Date = date,
                ID = Guid.NewGuid().ToString(),
                Mileage = mileage,
                Version = version
            };

            if (configurator != null)
            {
                configurator(@event);
            }

            return @event;
        }
    }
}
