using System.Diagnostics;
using System.Text.Json;

namespace FactoryCube.Core.Infrastructure.PythonRunner;

public class PythonRunnerService
{
    private readonly string _pythonExePath;
    private readonly string _scriptBasePath;
    private readonly ILogger<PythonRunnerService> _logger;

    public PythonRunnerService(IConfiguration config, ILogger<PythonRunnerService> logger)
    {
        _pythonExePath = config["Python:ExecutablePath"] ?? "python";
        _scriptBasePath = config["Python:ScriptPath"] ?? Path.Combine("..", "FactoryCube.Python");
        _logger = logger;
    }

    public async Task<(bool success, string output, string error)> RunAsync(
        string jobType,
        Dictionary<string, object> config,
        string inputPath,
        string outputPath,
        CancellationToken ct = default)
    {
        var configPath = Path.Combine(outputPath, "config.json");
        Directory.CreateDirectory(outputPath);
        await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }), ct);

        var psi = new ProcessStartInfo
        {
            FileName = _pythonExePath,
            Arguments = $"\"{Path.Combine(_scriptBasePath, "main.py")}\" --job-type {jobType} --config \"{configPath}\" --input \"{inputPath}\" --output \"{outputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _logger.LogInformation("Starting Python job: {JobType} with config {ConfigPath}", jobType, configPath);
        using var process = Process.Start(psi);
        if (process == null) throw new InvalidOperationException("Failed to start python process");

        var output = await process.StandardOutput.ReadToEndAsync(ct);
        var error = await process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
        {
            _logger.LogError("Python job failed: {Error}", error);
            return (false, output, error);
        }
        _logger.LogInformation("Python job completed: {JobType}", jobType);
        return (true, output, error);
    }
}
