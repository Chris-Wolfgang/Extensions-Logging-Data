#if NET8_0_OR_GREATER

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

/// <summary>
/// Asserts that the hot-path through <c>LogDbConnection</c> — the
/// IsEnabled fast-path that production call sites typically hit — does
/// not allocate. Locks the cheap branch in so a future refactor that
/// inadvertently allocates on every call (boxing, string interpolation,
/// LINQ, captured closures, etc.) surfaces here rather than in
/// production GC pressure.
///
/// Net8+ only because <see cref="GC.GetAllocatedBytesForCurrentThread"/>
/// is only reliable on the modern runtime. The unit project's wider TFM
/// matrix (net462–net7.0) still runs the rest of the suite — the
/// allocation assertion just sits out those frameworks.
/// </summary>
public class AllocationFreeHotPathTests
{
    private static readonly ILogger DisabledLogger = NullLogger.Instance;



    [Fact]
    public void LogDbConnection_with_disabled_logger_does_not_allocate()
    {
        var connection = new FakeDbConnection();

        // Warm-up: prime the JIT and tier-1 compilation so the measured
        // run isn't paying first-call codegen costs. Three iterations
        // matches the empirical guidance from the .NET runtime team's
        // micro-benchmarking notes.
        for (var i = 0; i < 3; i++)
        {
            DisabledLogger.LogDbConnection(connection);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
        {
            DisabledLogger.LogDbConnection(connection);
        }
        var after = GC.GetAllocatedBytesForCurrentThread();

        var allocatedBytes = after - before;

        Assert.True(
            allocatedBytes == 0,
            $"LogDbConnection with disabled logger allocated {allocatedBytes} bytes across 1000 calls; expected 0. " +
            "The IsEnabled fast-path is supposed to be allocation-free — a non-zero result means a recent refactor " +
            "introduced boxing, an interpolated string, a captured-variable closure, or a LINQ query in the early-out path.");
    }



    [Fact]
    public void LogDbConnection_with_disabled_logger_explicit_level_does_not_allocate()
    {
        var connection = new FakeDbConnection();

        // Same warm-up + measure pattern, but exercising the
        // explicit-level overload so a future divergence between
        // the two overloads' early-out cost surfaces here.
        for (var i = 0; i < 3; i++)
        {
            DisabledLogger.LogDbConnection(connection, LogLevel.Debug);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 1000; i++)
        {
            DisabledLogger.LogDbConnection(connection, LogLevel.Debug);
        }
        var after = GC.GetAllocatedBytesForCurrentThread();

        var allocatedBytes = after - before;

        Assert.True(
            allocatedBytes == 0,
            $"LogDbConnection(level=Debug) with disabled logger allocated {allocatedBytes} bytes across 1000 calls; expected 0.");
    }
}

#endif
