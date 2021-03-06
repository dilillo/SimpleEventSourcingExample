using System.Collections.Generic;
using System.Linq;

namespace SimpleEventSourcing
{
    /// <summary>
    /// Creates snapshots of car maintaince data by processing the event stream.
    /// </summary>
    public class CarViewProjector
    {
        public CarView Project(IEnumerable<CarEvent> events) => new CarView
        {
            TotalMainteances = events.Count(),
            OilLastChanged = events.OfType<OilChanged>().LastOrDefault()?.Date,
            AverageMaintenanceCost = events.Average(i => i.Cost)
        };
    }
}
