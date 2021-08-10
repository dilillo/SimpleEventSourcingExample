using System;
using System.Linq;
using Xunit;

namespace SimpleEventSourcing.UnitTests
{
    public class UnitTests
    {
        /// <summary>
        /// Tests that events are processed and projected without any pre-existing state
        /// </summary>
        [Fact]
        public void Test1()
        {
            var aggregate = new CarAggregate("Lambo");

            aggregate.OilChanged(new DateTime(2021, 1, 1), 3000, 50M, "5W-40");
            aggregate.BrakesServiced(new DateTime(2021, 3, 3), 6000, 250M);
            aggregate.BatteryTested(new DateTime(2021, 4, 4), 9000, 0M, 2d);

            Assert.Equal(3, aggregate.Version);
            Assert.Equal(9000, aggregate.Mileage);
            Assert.Equal(300M, aggregate.TotalMaintenceCost);

            var projector = new CarViewProjector();

            var view = projector.Project(aggregate.EventStream);

            Assert.Equal(3, view.TotalMainteances);
            Assert.Equal(new DateTime(2021, 1, 1), view.OilLastChanged);
            Assert.Equal(100M, view.AverageMaintenanceCost);
        }

        /// <summary>
        /// Checks that events are processed and projected with pre-existing state
        /// </summary>
        [Fact]
        public void Test2()
        {
            const string aggregateID = "Porshe";

            var aggregate = new CarAggregate(aggregateID);

            aggregate.Load(new CarEvent[]
            {
               CarEventFactory.Create<OilChanged>(aggregateID, 1, new DateTime(2021, 1, 1), 3000, 50M, i => i.OilType = "5W-40"),
               CarEventFactory.Create<BrakesServiced>(aggregateID, 2, new DateTime(2021, 3, 3), 6000, 250M)
            });

            aggregate.BatteryTested(new DateTime(2021, 4, 4), 9000, 0M, 2d);

            Assert.Equal(3, aggregate.Version);
            Assert.Equal(9000, aggregate.Mileage);
            Assert.Equal(300M, aggregate.TotalMaintenceCost);

            var projector = new CarViewProjector();

            var view = projector.Project(aggregate.EventStream);

            Assert.Equal(3, view.TotalMainteances);
            Assert.Equal(new DateTime(2021, 1, 1), view.OilLastChanged);
            Assert.Equal(100M, view.AverageMaintenanceCost);
        }

        /// <summary>
        /// Checks that events are processed and projected when only part of the event stream is needed
        /// </summary>
        [Fact]
        public void Test3()
        {
            const string aggregateID = "Porshe";

            var aggregate = new CarAggregate(aggregateID);

            aggregate.Load(new CarEvent[]
            {
               CarEventFactory.Create<OilChanged>(aggregateID, 1, new DateTime(2021, 1, 1), 3000, 50M, i => i.OilType = "5W-40"),
               CarEventFactory.Create<BrakesServiced>(aggregateID, 2, new DateTime(2021, 3, 3), 6000, 250M),
               CarEventFactory.Create<BatteryTested>(aggregateID, 3, new DateTime(2021, 4, 4), 3000, 50M, i => i.BatteryLevel = 2d)
            });

            var projector = new CarViewProjector();

            var view = projector.Project(aggregate.EventStream.Where(i => i.Date < new DateTime(2021, 3, 3)));

            Assert.Equal(1, view.TotalMainteances);
            Assert.Equal(new DateTime(2021, 1, 1), view.OilLastChanged);
            Assert.Equal(50M, view.AverageMaintenanceCost);
        }

        /// <summary>
        /// Checks to validate an exception is thrown when invalid mileage is provided
        /// </summary>
        [Fact]
        public void Test4()
        {
            const string aggregateID = "Ferrari";

            var aggregate = new CarAggregate(aggregateID);

            aggregate.Load(new CarEvent[]
            {
               CarEventFactory.Create<OilChanged>(aggregateID, 1, new DateTime(2021, 1, 1), 3000, 50M, i => i.OilType = "5W-40")
            });

            Assert.Throws<CarException>(() => aggregate.BatteryTested(new DateTime(2021, 4, 4), 1000, 0M, 2d));
        }
    }
}
