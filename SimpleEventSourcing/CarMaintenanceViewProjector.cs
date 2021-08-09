using System.Collections.Generic;
using System.Linq;

namespace SimpleEventSourcing
{
    public interface ICarMaintenanceViewProjector
    {
        CarMaintenanceView Project(IEnumerable<CarMaintenanceEvent> events);
    }

    /// <summary>
    /// Creates snapshots of car maintaince data by processing the event stream.
    /// </summary>
    public class CarMaintenanceViewProjector : ICarMaintenanceViewProjector
    {
        public CarMaintenanceView Project(IEnumerable<CarMaintenanceEvent> events) => new CarMaintenanceView
        {
            TotalMainteances = events.Count(),
            OilLastChanged = events.LastOrDefault(i => i.CarMaintenceType == CarMaintenceTypes.OilChanged)?.Date,
            AverageMaintenanceCost = events.Average(i => i.Cost)
        };
    }
}
