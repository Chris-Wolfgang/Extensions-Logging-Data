using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Wolfgang.Extensions.Logging.Data.Tests.Docs;

/// <summary>
/// Compiles every <c>&lt;example&gt;&lt;code&gt;</c> block found in the XML-doc comments of
/// the source, catching "documentation rot" — an example that stops compiling because the
/// API it demonstrates was renamed, removed, or changed. Each block is wrapped in a minimal
/// async harness (ambient <c>logger</c> / <c>connection</c> / <c>connectionString</c> /
/// <c>command</c> are injected only when a block uses but does not declare them) and compiled
/// with Roslyn against the same reference set the test process runs on.
/// </summary>
public class DocExampleCompilationTests
{
    private static readonly (string Name, string Declaration)[] AmbientCandidates =
    {
        ("logger", "Microsoft.Extensions.Logging.ILogger logger = null!;"),
        ("connectionString", "string connectionString = null!;"),
        ("connection", "System.Data.Common.DbConnection connection = null!;"),
        ("command", "System.Data.Common.DbCommand command = null!;"),
    };

    public static IEnumerable<object[]> Examples()
    {
        var srcDir = FindSourceDirectory();
        foreach (var file in Directory.EnumerateFiles(srcDir, "*.cs", SearchOption.AllDirectories))
        {
            // Skip build outputs — bin/obj can contain generated .cs (and copied
            // sources) that would be scanned needlessly.
            if (IsUnderBuildOutput(file))
            {
                continue;
            }

            var text = File.ReadAllText(file);
            var i = 0;
            foreach (var code in ExtractCodeBlocks(text))
            {
                i++;
                yield return new object[] { $"{Path.GetFileName(file)}#{i}", code };
            }
        }
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DocExample_compiles(string id, string code)
    {
        var program = BuildProgram(code);

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Where(p => p.Length > 0)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        var compilation = CSharpCompilation.Create
        (
            "DocExamples",
            new[] { CSharpSyntaxTree.ParseText(program) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable)
        );

        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.ToString())
            .ToList();

        Assert.True
        (
            errors.Count == 0,
            $"Doc example {id} does not compile:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}{Environment.NewLine}--- generated ---{Environment.NewLine}{program}"
        );
    }

    [Fact]
    public void At_least_one_example_was_discovered()
    {
        // Guards against the extraction silently finding nothing (e.g. a regex or path
        // regression) — which would make DocExample_compiles a vacuous green.
        Assert.NotEmpty(Examples());
    }

    private static string BuildProgram(string snippet)
    {
        var declared = new HashSet<string>(
            Regex.Matches(snippet, @"\b(?:using\s+)?var\s+(\w+)\s*=")
                 .Select(m => m.Groups[1].Value));

        var ambient = new StringBuilder();
        foreach (var (name, declaration) in AmbientCandidates)
        {
            var usesIt = Regex.IsMatch(snippet, $@"\b{name}\b");
            if (usesIt && !declared.Contains(name))
            {
                ambient.Append("        ").Append(declaration).Append('\n');
            }
        }

        return
            "using System;\n" +
            "using System.Collections.Generic;\n" +
            "using System.Data.Common;\n" +
            "using System.Data.SqlClient;\n" +
            "using System.Threading.Tasks;\n" +
            "using Microsoft.Extensions.Logging;\n" +
            "using Wolfgang.Extensions.Logging.Data;\n" +
            "internal class __DocExampleHarness\n{\n" +
            "    private async Task __RunAsync()\n    {\n" +
            ambient +
            snippet + "\n" +
            "        await Task.CompletedTask;\n" +
            "    }\n}\n";
    }

    private static IEnumerable<string> ExtractCodeBlocks(string fileText)
    {
        // Match a <code> ... </code> region inside a run of `///` comment lines.
        foreach (Match m in Regex.Matches(fileText, @"///\s*<code>\s*\r?\n(?<body>.*?)///\s*</code>", RegexOptions.Singleline))
        {
            var body = m.Groups["body"].Value;
            var lines = body.Split('\n')
                .Select(l => l.TrimEnd('\r'))
                .Select(StripDocPrefix)
                .ToList();
            var code = WebUtility.HtmlDecode(string.Join("\n", lines)).Trim();
            if (code.Length > 0)
            {
                yield return code;
            }
        }
    }

    private static string StripDocPrefix(string line)
    {
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith("///", StringComparison.Ordinal))
        {
            trimmed = trimmed.Substring(3);
            if (trimmed.StartsWith(" ", StringComparison.Ordinal))
            {
                trimmed = trimmed.Substring(1);
            }
            return trimmed;
        }
        return line;
    }

    private static bool IsUnderBuildOutput(string path)
    {
        var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return parts.Any(p => string.Equals(p, "bin", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(p, "obj", StringComparison.OrdinalIgnoreCase));
    }

    private static string FindSourceDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "Wolfgang.Extensions.Logging.Data");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate src/Wolfgang.Extensions.Logging.Data walking up from {AppContext.BaseDirectory}");
    }
}
