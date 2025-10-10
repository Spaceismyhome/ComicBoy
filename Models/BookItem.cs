using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicReaderApp.Models
{
    public class BookItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public  string Title { get; set; }
        public  string? FilePath { get; set; }
        public  string? FileType { get; set; }

    }

    public class BookCollection
    {
        public ObservableCollection<BookItem> Books { get; set; } = new();
    }
}
