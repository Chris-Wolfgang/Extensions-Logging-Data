// Polyfill for RequiresUnreferencedCodeAttribute, which the .NET 5+ BCL provides but
// the older target frameworks (net462, netstandard2.0, netstandard2.1) do not. The IL
// trimmer / Native AOT compiler match this attribute by full type name, so an internal
// definition with the canonical name is recognized exactly as the framework type would
// be. On net5.0+ (incl. net10.0) the real BCL type is used and this file compiles away.
#if !NET5_0_OR_GREATER

#pragma warning disable MA0048 // File name must match type name — this is a framework polyfill.

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Indicates that the marked method uses functionality (here, reflection over a runtime
    /// type's members) that cannot be statically analysed by the trimmer, so calling it from
    /// a trimmed / Native AOT application is not guaranteed to be safe. Polyfill of the .NET 5+
    /// framework attribute for older target frameworks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    [ExcludeFromCodeCoverage] // Attribute-only polyfill — no executable code to cover.
    internal sealed class RequiresUnreferencedCodeAttribute : Attribute
    {
        public RequiresUnreferencedCodeAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public string? Url { get; set; }
    }
}

#pragma warning restore MA0048

#endif
