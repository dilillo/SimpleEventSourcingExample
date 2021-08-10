using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleEventSourcing
{
    public interface ICarAggregate
    {
        string AggregateID { get; }

        CarEvent[] EventStream { get; }

        int Mileage { get; }

        decimal TotalMaintenceCost { get; }

        int Version { get; }

        void Load(IEnumerable<CarEvent> eventStream);

        void BatteryTested(DateTime date, int mileage, decimal cost);

        void BrakesServiced(DateTime date, int mileage, decimal cost);

        void OilChanged(DateTime date, int mileage, decimal cost);
    }

    /// <summary>
    /// Handles business logic by processing a stream of events.
    /// </summary>
    public class CarAggregate : ICarAggregate
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

        public void BatteryTested(DateTime date, int mileage, decimal cost)
        {
            CheckMileage(mileage);

            var @event = CreateCarEvent(CarEventTypes.BatteryTested, date, mileage, cost);

            Mutate(@event);

            _eventStream.Add(@event);
        }

        public void BrakesServiced(DateTime date, int mileage, decimal cost)
        {
            CheckMileage(mileage);

            var @event = CreateCarEvent(CarEventTypes.BrakesServiced, date, mileage, cost);

            Mutate(@event);

            _eventStream.Add(@event);
        }

        public void OilChanged(DateTime date, int mileage, decimal cost)
        {
            CheckMileage(mileage);

            CheckOil();

            var @event = CreateCarEvent(CarEventTypes.OilChanged, date, mileage, cost);

            Mutate(@event);

            _eventStream.Add(@event);
        }

        private void CheckOil()
        {
            if (_eventStream.LastOrDefault()?.CarMaintenceType == CarEventTypes.OilChanged)
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

        private CarEvent CreateCarEvent(CarEventTypes type, DateTime date, int mileage, decimal cost) => new CarEvent(AggregateID, Version + 1, type, date, mileage, cost);

        private void Mutate(CarEvent @event)
        {
            Version = @event.Version;
            Mileage = @event.Mileage;
            TotalMaintenceCost += @event.Cost;
        }
    }
}
