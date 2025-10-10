using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace ComicReaderApp.Views;

public partial class ReaderPage : ContentPage
{
    private readonly string _filePath;

    private bool _isPaused = false;
    private bool _isReading = false;
    private bool _isStopped = false;
    private int _currentPage = 1;
    private int _currentChunkIndex = 0;
    List<string> _paragraphs = new List<string>();
    string rawText;
    public ReaderPage(string filePath)
    {
        InitializeComponent();
        _filePath = filePath;
        Title = Path.GetFileNameWithoutExtension(filePath);

        // Display the PDF file
        PdfWebView.Source = new UrlWebViewSource
        {
            Url = Path.GetFullPath(filePath)
        };
    }

    private async void OnReadResumeClicked(object sender, EventArgs e)
    {
        if (_isReading && !_isPaused)
        {
            return;
        }

        
       

        if (!_isReading) {
            string result = await DisplayPromptAsync(
            "Start Reading",
            "Enter page number to start from:",
            initialValue: "1",
            keyboard: Keyboard.Numeric);
            
            

            if (!int.TryParse(result, out _currentPage) || _currentPage < 1)
            {
                await DisplayAlert("Invalid", "Enter a valid page number.", "OK");
                return;
            }


            _currentChunkIndex = 0;
        }
        _isPaused = false;
        _isReading = true;

        await ReadPdfPageByPage(_filePath);
    }

    private void OnPauseClicked(object sender, EventArgs e)
    {
        _isPaused = true;
    }

    string ExtractPdfText(string filePath)
    {
        var sb = new StringBuilder();
        using (var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                sb.AppendLine(page.Text);
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }


    string NormalizePdfText(string rawText)
    {
        // Remove page numbers
        rawText = Regex.Replace(rawText, @"\n\s*\d+\s*\n", "\n");

        // Remove multiple new lines
        rawText = Regex.Replace(rawText, @"\n{2,}", "\n\n");

        // Join broken lines
        rawText = Regex.Replace(rawText, @"(?<![.!?])\n", " ");

        return rawText.Trim();
    }

    private async Task ReadPdfPageByPage(string filePath)
{
    using var document = PdfDocument.Open(filePath);
    int pageIndex = 1;

    foreach (var page in document.GetPages())
    {
        if (pageIndex < _currentPage)
        {
            pageIndex++;
            continue;
        }

        string text = page.Text;
        if (!string.IsNullOrWhiteSpace(text))
        {
            await SpeakPage(text);
        }
            string rawText = ExtractPdfText(_filePath);
            string cleanedText = NormalizePdfText(rawText);
            _paragraphs = cleanedText.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _currentPage++;
        _currentChunkIndex = 0;

        if (_isPaused)
            return;

        pageIndex++;
    }

    _isReading = false;
}
    private async Task SpeakPage(string text)
    {
        _isStopped = false;
        _isPaused = false;

        const int chunkSize = 300;
        int chunkCount = (int)Math.Ceiling((double)text.Length / chunkSize);

        for (int i = _currentChunkIndex; i < chunkCount; i++)
        {
            if (_isStopped)
            {
                _currentChunkIndex = 0; // reset
                return;
            }

            if (_isPaused)
            {
                _currentChunkIndex = i; // save position
                return;
            }

            int start = i * chunkSize;
            int length = Math.Min(chunkSize, text.Length - start);
            string chunk = text.Substring(start, length);

            await TextToSpeech.SpeakAsync(chunk);
        }

        _currentChunkIndex = 0;
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        _isStopped = true;
        _isPaused = false;
        _currentChunkIndex = 0;
    }



}
