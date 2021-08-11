using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEventSourcing
{
    /// <summary>
    /// Handles business logic by processing a stream of events.
    /// </summary>
    public class CarAggregate
    {
        public CarAggregate(string aggregateID)
        {
            AggregateID = aggregateID;
        }

        public string AggregateID { get; private set; }

        public int Version { get; private set; }

        public int Mileage { get; private set; }

        public decimal TotalMaintenceCost { get; private set; }

        private readonly List<CarEvent> _eventStream = new List<CarEvent>();

        public CarEvent[] EventStream
        {
            get => _eventStream.ToArray();
        }

        public void Load(IEnumerable<CarEvent> eventStream)
        {
            _eventStream.AddRange(eventStream);

            foreach (var @event in eventStream)
            {
                Mutate(@event);
            }
        }

        public void BatteryTested(DateTime date, int mileage, decimal cost, double batteryLevel)
        {
            CheckMileage(mileage);

            var @event = CarEventFactory.Create<BatteryTested>(AggregateID, Version + 1, date, mileage, cost, i => i.BatteryLevel = batteryLevel);

            Mutate(@event);

            _eventStream.Add(@event);
        }

        public void BrakesServiced(DateTime date, int mileage, decimal cost)
        {
            CheckMileage(mileage);

            var @event = CarEventFactory.Create<BrakesServiced>(AggregateID, Version + 1, date, mileage, cost);

            Mutate(@event);

            _eventStream.Add(@event);
        }

        public void OilChanged(DateTime date, int mileage, decimal cost, string oilType)
        {
            CheckMileage(mileage);

            CheckOil();

            var @event = CarEventFactory.Create<OilChanged>(AggregateID, Version + 1, date, mileage, cost, i => i.OilType = oilType);

            Mutate(@event);

            _eventStream.Add(@event);
        }

        private void CheckOil()
        {
            if (_eventStream.LastOrDefault() is OilChanged)
            {
                throw new CarException("Didn't you just have the oil changed?");
            }
        }

        private void CheckMileage(int mileage)
        {
            if (mileage < Mileage)
            {
                throw new CarException("No winding back the odometer!");
            }
        }

        private void Mutate(CarEvent @event)
        {
            Version = @event.Version;
            Mileage = @event.Mileage;
            TotalMaintenceCost += @event.Cost;
        }
    }
}
