#if NET5_0

using System.Reflection;
using System.Runtime.Versioning;
using Xunit;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

/// <summary>
/// Pins which build of the library the net5.0 slot exercises. Nearest-asset resolution would
/// give net5.0 the <c>netstandard2.1</c> build, so the project file points this slot at
/// <c>netstandard2.0</c> instead (ADR-0004). If that wiring is lost, the run silently falls
/// back to <c>netstandard2.1</c> and <c>netstandard2.0</c> ships untested again; this test
/// fails first.
/// </summary>
public class TestedBuildAssetTests
{
    [Fact]
    public void LibraryAssembly_when_tests_run_on_net5_0_is_the_netstandard2_0_build()
    {
        var targetFramework = typeof(DbCommandLoggerExtensions)
            .Assembly
            .GetCustomAttribute<TargetFrameworkAttribute>()!;

        Assert.Equal
        (
            ".NETStandard,Version=v2.0",
            targetFramework.FrameworkName
        );
    }
}

#endif
