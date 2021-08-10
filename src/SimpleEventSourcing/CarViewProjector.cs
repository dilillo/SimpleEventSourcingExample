using System.Collections.Generic;
using System.Linq;

namespace SimpleEventSourcing
{
    public interface ICarMaintenanceViewProjector
    {
        CarView Project(IEnumerable<CarEvent> events);
    }

    /// <summary>
    /// Creates snapshots of car maintaince data by processing the event stream.
    /// </summary>
    public class CarViewProjector : ICarMaintenanceViewProjector
    {
        public CarView Project(IEnumerable<CarEvent> events) => new CarView
        {
            TotalMainteances = events.Count(),
            OilLastChanged = events.LastOrDefault(i => i.CarMaintenceType == CarEventTypes.OilChanged)?.Date,
            AverageMaintenanceCost = events.Average(i => i.Cost)
        };
    }
}
