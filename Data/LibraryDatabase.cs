using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ComicReaderApp.Models;

namespace ComicReaderApp.Data
{
    public class LibraryDatabase
    {
        readonly SQLiteAsyncConnection _database;
        public LibraryDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<BookItem>().Wait();
        }
        public Task<List<BookItem>> GetBooksAsync()
        {
            return _database.Table<BookItem>().ToListAsync();
        }
        public Task<int> SaveBookAsync(BookItem book)
        {
           if (book.Id != 0)
           {
               return _database.UpdateAsync(book);
           }
           else
           {
               return _database.InsertAsync(book);
            }
        }
    }
}
