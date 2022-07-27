using Bookstore.Books.Common;
using Bookstore.Books.Common.Models;
using Bookstore.Books.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Books.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<Book> _booksCollection;
        private readonly BookStoreDatabaseSettings _settings;

        public BookRepository(IOptions<BookStoreDatabaseSettings> options)
        {
            _settings = options.Value;
            _mongoClient = new MongoClient(_settings.ConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(_settings.DatabaseName);
            _booksCollection = _mongoDatabase.GetCollection<Book>(_settings.CollectionName);
        }

        public async Task CreateBook(Book newBook) => await _booksCollection.InsertOneAsync(newBook);

        public async Task DeleteBook(string id) => await _booksCollection.DeleteOneAsync(x => x.Id == id);
        
        public async Task<List<Book>> GetAllBooks() => await _booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book> GetBookById(string id) => await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task UpdateBook(string id, Book book) => await _booksCollection.ReplaceOneAsync(x => x.Id == id, book);
    }
}
