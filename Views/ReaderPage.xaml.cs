using System.IO;
using Microsoft.Maui.Controls; // Ensure this is present for WebView

namespace ComicReaderApp.Views;

public partial class ReaderPage : ContentPage
{
    public ReaderPage(string filePath)
    {
        InitializeComponent();

        // Display the PDF using WebView
        if (File.Exists(filePath))
        {
            // WebView does not have a DocumentSource property.
            // Instead, set the Source property to a file URL.
            PdfViewer.Source = new UrlWebViewSource
            {
                Url = $"file://{filePath}"
            };
        }
        else
        {
            DisplayAlert("Error", "File not found!", "OK");
        }
    }
}
