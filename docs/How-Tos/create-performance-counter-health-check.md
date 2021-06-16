# Create a new Performance Counter
First read the [Functionality-Documentation/health-checks-and-performance-counters](../Functionality-Documentation/health-checks-and-performance-counters.md) guide to the current Health Check and Performance Counter behaviour for context.

## Steps to create a new Performance Counter
- Update the Middleware to record a new DataPoint metric (if required). 
  This must be a concrete implementation of the IDataPoint interface
- Add a new DataPointType to the enum even if re-using an existing metric measurement. 
  This enum type is used throughout the process to determine what action to take, so if you need a new Performance counter, 
  you need a new type here
- The PerformanceCounterVisitor is where the raw DataPoint metrics are added to a '2nd-stage' refined Performance Counter dictionary. 
  If a new IDataPoint implementation is added you will need a new Visit method. 
  If you wish to only add a new PerformanceCounter utilising an existing DataPoint, 
  you will need to extend the relevant existing Visit method to add to the dictionary entry automatically created in the 
  backing store contained in this class for your new DataPointType Key.
- Add to the PerformanceCounterMetricsFactory for your new DataPointType if any calculations are required prior 
  to storing the PerformanceCounter. This is not required.

## Steps to add a new Health Check
- To add a new Health Check (e.g. against the new Performance Counter you created above), 
  just need to add a new entry in the PerformanceCounterHealthCheckStatusFactory for your new DataPointType. 


