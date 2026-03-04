using System.Diagnostics;

namespace KillPort;

public static class Handler {
    private static int GetPIDOnListeningPort(int port) {
        var processStartInfo = new ProcessStartInfo {
            FileName = "cmd.exe",
            Arguments = $"/c netstat -ano | findstr :{port}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo)!;
        var processOutput = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        var line = processOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(str => str.Contains("LISTENING", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(line)) return 0;

        // netstat: Proto LocalAddress ForeignAddress State PID
        // Ex: TCP 0.0.0.0:4200 0.0.0.0:0 LISTENING 12345
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return !int.TryParse(parts.Last(), out var pid) ? 0 : pid;
    }

    public static bool KillPort(int port) {
        var pid = GetPIDOnListeningPort(port);
        if (pid == 0) return false;

        try {
            var process = Process.GetProcessById(pid);
            process.Kill();
            return true;
        }
        catch {
            return false;
        }
    } 
}