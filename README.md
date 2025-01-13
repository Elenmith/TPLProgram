# Numerical Integration with Parallel Computing and Plotting

This project demonstrates the numerical integration of mathematical functions using the trapezoidal rule with partitioned parallel computing. It also includes a functionality for plotting the integrated functions using the ScottPlot library.

---

## Features
1. **Function Definitions:**
   - Implements three different mathematical functions:
     - `y = 2x + 2x^2`
     - `y = 2x^2 + 3`
     - `y = 3x^2 + 2x - 3`
   
2. **Numerical Integration:**
   - Uses the trapezoidal rule for integration.
   - Supports partitioning of the integration interval to leverage parallel computing for efficiency.

3. **Plotting:**
   - Generates a plot of the selected function over a given range using the ScottPlot library.
   - Saves the plot as a PNG file on the desktop.

4. **Parallel Computing:**
   - Utilizes `Parallel.ForEach` with custom partitioning for efficient calculation of the integration in multiple threads.

---

## Requirements
- .NET Framework or .NET Core
- ScottPlot library for plotting
- A system capable of running C# console applications

---

## How It Works
### Main Workflow
1. Select a predefined function (`Function1`, `Function2`, or `Function3`).
2. Define the integration range, number of intervals, and number of partitions.
3. Perform the integration using `RunParallelPartitions`:
   - The range is divided into equal partitions.
   - Each partition is processed in parallel to calculate the integration result.
4. Plot the function using the `Ploter` method:
   - Saves the plot to a specified file on the desktop.

### Error Handling
- Invalid inputs for file names or unauthorized directory access are handled gracefully.
- Ensures thread safety when updating shared results in parallel tasks.

---

## Code Example
### Main Execution
```
csharp
static void Main(string[] args)
{
    IFunction function = new Function2();

    double start = 1;
    double end = 35;
    int intervals = 10;
    int partitionCount = 10;

    RunParallelPartitions(function, start, end, intervals, partitionCount);
}
```

## Future Enhancements
* Add support for user-defined functions.
* Implement more advanced numerical integration methods.
* Introduce a graphical user interface (GUI) for ease of use.
