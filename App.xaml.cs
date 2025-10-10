using ComicReaderApp.Data;
using ComicReaderApp.Views;

namespace ComicReaderApp
{
    public partial class App : Application
    {
        public static LibraryDatabase database;
        
        public App()
        {
            InitializeComponent();
            var path = Path.Combine(FileSystem.AppDataDirectory, "MyLibrary.db3");
            database = new LibraryDatabase(path);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new MainPage()));
        }
    }
}