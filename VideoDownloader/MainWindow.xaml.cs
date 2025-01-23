using System.IO;
using System.Windows;
using System.Windows.Controls;
using Brushes = System.Windows.Media.Brushes;

namespace VideoDownloader;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        BrowseButton.Click += BrowseButton_Click;
        DownloadButton.Click += DownloadButton_Click;
        
        // Set default destination path to user's Videos folder
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        
        // Create the directory if it doesn't exist
        if (!Directory.Exists(defaultPath))
        {
            Directory.CreateDirectory(defaultPath);
        }
        
        DestinationPathTextBox.Text = defaultPath;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select destination folder for video download",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            DestinationPathTextBox.Text = dialog.SelectedPath;
            IsInputValid();
        }
    }

    private async void DownloadButton_Click(object sender, RoutedEventArgs e)
    {
        LogTextBox.Clear();
        var isInputValid = IsInputValid();
        if (!isInputValid)
        {
            LogError("Invalid input. Please check the URL and destination path.");
            return;
        }

        try
        {
            DownloadButton.IsEnabled = false;
            await StartDownloadAsync(UrlTextBox.Text, DestinationPathTextBox.Text);
            LogMessage("Download completed successfully!");
        }
        catch (Exception ex)
        {
            LogError($"Download failed: {ex.Message}");
        }
        finally
        {
            DownloadButton.IsEnabled = true;
        }
    }

    private void LogError(string message)
    {
        LogTextBox.AppendText($"ERROR: {message}\n");
        LogTextBox.ScrollToEnd();
    }

    private void LogMessage(string message)
    {
        LogTextBox.AppendText($"{message}\n");
        LogTextBox.ScrollToEnd();
    }
    
    private bool IsInputValid()
    {
        var isUrlValid = IsUrlValid(UrlTextBox.Text);
        var isDestinationPathValid = IsDestinationPathValid(DestinationPathTextBox.Text);
        UrlTextBox.BorderBrush = isUrlValid ? Brushes.Black : Brushes.Red;
        DestinationPathTextBox.BorderBrush = isDestinationPathValid ? Brushes.Black : Brushes.Red;
        
        return isUrlValid && isDestinationPathValid;
    }

    private bool IsDestinationPathValid(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return Directory.Exists(text);
    }

    private bool IsUrlValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return false;
        }
        return true;
    }
}