namespace OGL.Gen2.Simulator;

/// <summary>
/// Small, strict INI reader for the initial Gen2 simulator bootstrap.
/// It does not imply compatibility with all OpenSimulator INI directives.
/// </summary>
public sealed class IniReader
{
    private readonly Dictionary<string, Dictionary<string, string>> _sections =
        new(StringComparer.OrdinalIgnoreCase);

    public static IniReader Load(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("INI file not found.", path);
        var ini = new IniReader();
        var section = "";
        var lineNumber = 0;
        foreach (var raw in File.ReadLines(path))
        {
            lineNumber++;
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#')) continue;
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                section = line[1..^1].Trim();
                if (section.Length == 0) throw new FormatException($"Empty section in {path}:{lineNumber}");
                if (!ini._sections.TryAdd(section, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)))
                    throw new FormatException($"Duplicate section {section} in {path}:{lineNumber}");
                continue;
            }

            var split = line.IndexOf('=');
            if (split <= 0 || section.Length == 0)
                throw new FormatException($"Invalid INI assignment at {path}:{lineNumber}");
            var key = line[..split].Trim();
            var value = line[(split + 1)..].Trim();
            if (key.Length == 0 || !ini._sections[section].TryAdd(key, value.Trim('"')))
                throw new FormatException($"Duplicate or empty key at {path}:{lineNumber}");
        }
        return ini;
    }

    public string Require(string section, string key)
    {
        if (!_sections.TryGetValue(section, out var values) ||
            !values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            throw new FormatException($"Required INI value missing: [{section}] {key}");
        return value;
    }

    public string Get(string section, string key, string fallback) =>
        _sections.TryGetValue(section, out var values) && values.TryGetValue(key, out var result)
            ? result
            : fallback;
}
