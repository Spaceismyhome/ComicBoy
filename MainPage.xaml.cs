using ComicReaderApp.Models;
using ComicReaderApp.Views;
using Microsoft.Maui.Storage;
using ComicReaderApp.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace ComicReaderApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<BookItem> Books { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            LoadBooks();
            BooksCollection.ItemsSource = Books;
        }

        async void LoadBooks()
        {
           var booksFromDb = await App.database.GetBooksAsync();
              Books.Clear();
              foreach (var book in booksFromDb)
                {
                    Books.Add(book);
            }
        }
        private async void OnAddBookClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync();
                        if (result != null) 
                        {
                            var Book = new BookItem
                            {
                                Title = Path.GetFileNameWithoutExtension(result.FileName),
                                FilePath = result.FullPath,
                                FileType = Path.GetExtension(result.FileName)
                            };
                            await App.database.SaveBookAsync(Book);
                            Books.Add(Book);
            }
        }

        private async void OnBookSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is BookItem selectedBook)
            {
                await Navigation.PushAsync(new ReaderPage(selectedBook.FilePath));
            }

            ((CollectionView)sender).SelectedItem = null;
        }

       
    }
}

