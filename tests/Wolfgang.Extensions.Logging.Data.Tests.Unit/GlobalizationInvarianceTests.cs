using System;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Logging;
using Wolfgang.Extensions.Logging.Data.Tests.Unit.TestHelpers;

namespace Wolfgang.Extensions.Logging.Data.Tests.Unit;

/// <summary>
/// Verifies that the library's redaction and logging output is
/// culture-invariant. The historic foot-guns this guards against:
///
///   - The Turkish-I problem: <c>"PASSWORD".ToLower()</c> under tr-TR
///     yields "passwoıd" (dotless 'i'), so a culture-sensitive
///     case-fold over the key list would fail to redact 'PASSWORD' in
///     a Turkish-locale process.
///   - <see cref="DbConnectionStringBuilder"/> is documented to compare
///     keys ordinal-ignore-case rather than current-culture, but a
///     hypothetical future refactor that introduced a
///     <c>StringComparison.CurrentCultureIgnoreCase</c> would regress
///     silently in non-Turkish-locale test runs.
///   - Numeric formatting in the structured-log payload: under
///     de-DE the comma is the decimal separator, so any int.ToString()
///     that picked up CurrentCulture would emit "1.000" instead of
///     "1000" for ConnectionTimeout when written by a culture-aware
///     sink.
///
/// The matrix mirrors the canonical Wolfgang.* globalization matrix
/// (tr-TR, de-DE, zh-CN, ar-SA, ja-JP) plus invariant.
/// </summary>
public class GlobalizationInvarianceTests
{
    [Theory]
    [InlineData("tr-TR")]   // Turkish-I problem
    [InlineData("de-DE")]   // comma decimal separator
    [InlineData("zh-CN")]   // CJK + non-ASCII codepage
    [InlineData("ar-SA")]   // RTL + non-Gregorian calendar
    [InlineData("ja-JP")]   // CJK + non-Gregorian calendar
    [InlineData("")]        // invariant
    public void RedactConnectionString_removes_password_under_any_culture(string cultureName)
    {
        using var _ = WithCurrentCulture(cultureName);

        var redacted = DbConnectionLoggerExtensions.RedactConnectionString(
            "Server=foo;User ID=bob;Password=hunter2");

        Assert.DoesNotContain("hunter2", redacted, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("bob", redacted, StringComparison.OrdinalIgnoreCase);
    }



    [Theory]
    [InlineData("tr-TR", "PASSWORD")]   // critical case: PASSWORD with capital I → tr-TR lower is wrong
    [InlineData("tr-TR", "Password")]
    [InlineData("tr-TR", "password")]
    [InlineData("tr-TR", "PWD")]
    [InlineData("tr-TR", "Pwd")]
    [InlineData("tr-TR", "pwd")]
    public void RedactConnectionString_handles_all_password_key_casings_under_turkish_locale(string cultureName, string keyCasing)
    {
        using var _ = WithCurrentCulture(cultureName);

        var redacted = DbConnectionLoggerExtensions.RedactConnectionString($"Server=foo;{keyCasing}=secret");

        Assert.DoesNotContain("secret", redacted, StringComparison.OrdinalIgnoreCase);
    }



    [Theory]
    [InlineData("tr-TR")]
    [InlineData("de-DE")]
    [InlineData("ja-JP")]
    [InlineData("")]
    public void LogDbConnection_emits_invariant_connection_timeout_value_regardless_of_culture(string cultureName)
    {
        using var _ = WithCurrentCulture(cultureName);

        var logger = new RecordingLogger();
        var connection = new FakeDbConnection { ConnectionTimeoutValue = 12345 };

        logger.LogDbConnection(connection);

        var entry = Assert.Single(logger.Entries);

        // The structured-log slot must hold the underlying int (not a
        // culture-formatted string). A culture-aware sink rendering it
        // later may add separators, but the library itself MUST emit
        // the raw value so structured-log consumers (Application
        // Insights, Elastic, etc.) see the same number across all
        // locales.
        Assert.Equal(12345, entry.GetValue("ConnectionTimeout"));
    }



    private static IDisposable WithCurrentCulture(string cultureName)
    {
        var culture = string.IsNullOrEmpty(cultureName)
            ? CultureInfo.InvariantCulture
            : new CultureInfo(cultureName);

        return new CultureScope(culture);
    }



    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previousCulture;
        private readonly CultureInfo _previousUiCulture;

        public CultureScope(CultureInfo culture)
        {
            _previousCulture   = Thread.CurrentThread.CurrentCulture;
            _previousUiCulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture   = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture   = _previousCulture;
            Thread.CurrentThread.CurrentUICulture = _previousUiCulture;
        }
    }
}
