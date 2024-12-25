using System.Diagnostics;
using System.Text;

namespace VideoDownloader;

public partial class MainWindow
{
    private Process? _currentProcess;
    
    private async Task StartDownloadAsync(string url, string destinationPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = $"\"{url}\" -P \"{destinationPath}\" -k",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        _currentProcess = new Process { StartInfo = startInfo };

        _currentProcess.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                Dispatcher.Invoke(() => LogMessage(args.Data));
            }
        };

        _currentProcess.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                Dispatcher.Invoke(() => LogError(args.Data));
            }
        };

        try
        {
            _currentProcess.Start();
            _currentProcess.BeginOutputReadLine();
            _currentProcess.BeginErrorReadLine();
            await _currentProcess.WaitForExitAsync();

            if (_currentProcess.ExitCode != 0)
            {
                throw new Exception($"yt-dlp exited with code {_currentProcess.ExitCode}");
            }
        }
        finally
        {
            _currentProcess?.Dispose();
            _currentProcess = null;
        }
    }
} 