type: internal

The `System.Diagnostics.CodeAnalysis` trim-annotation polyfill now uses a file-scoped namespace, so the ReSharper `CheckNamespace` suppression above it also applies when InspectCode runs on Linux; no code or API change.
