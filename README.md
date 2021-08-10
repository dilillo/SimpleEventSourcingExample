# Overview

This example code demonstrates how Event Sourcing can be implemented in its most basic form.  In it the `CarAggregate` accomplishes rudimentary business logic by processing an Event Stream.  Also, a `CarViewProjector` consumes that Event Stream to project a `CarView`.  The `CarView` is meant as an example of the flexibility that this approach affords the application in terms of the data it can provide to the consumer.  This could change over time or new views could be introduced to serve different needs.

# Getting Started

1. Clone the repo
1. Open the solution in Visual Studio
1. Run the Unit Tests found in the `src/SimpleEventSourcing.UnitTests/UnitTests.cs`

# Notes 
1. A more advanced implementation demonstrating how views can be projected in realtime, predicted in realtime, or projected later can be found in the [CarTracker](https://github.com/dilillo/CarTracker) repo
2. Awesome talk about how they levearged [Event Sourcing at Wix](https://medium.com/wix-engineering/watch-event-sourcing-at-wix-problems-and-solutions-maxim-kolotilkin-6b06c440c0aa) with some really good discussion of the need to consider data consumption patterns and how to handle them