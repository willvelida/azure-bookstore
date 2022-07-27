using Bookstore.Books.Common.Models;
using Bookstore.Books.Repository.Interfaces;
using Bookstore.Books.Services.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Books.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        {
            _bookRepository=bookRepository;
            _logger=logger;
        }

        public async Task CreateBook(BookRequestObject book)
        {
            try
            {
                _logger.LogInformation($"Creating Book: {book.BookName}");
                var newBook = new Book
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    BookName = book.BookName,
                    Author = book.Author,
                    Category = book.Category,
                    Price = book.Price
                };
                await _bookRepository.CreateBook(newBook);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteBook(string id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete Book Id: {id}");
                await _bookRepository.DeleteBook(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(DeleteBook)}: {ex.Message}");
                throw;
            }
        }

        public async Task<Book> GetBookById(string id)
        {
            try
            {
                _logger.LogInformation($"Attempting to retrieve Book Id: {id}");
                return await _bookRepository.GetBookById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetBookById)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Book>> GetBooks()
        {
            try
            {
                _logger.LogInformation($"Getting all books");
                return await _bookRepository.GetAllBooks();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetBooks)}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateBook(string id, BookRequestObject book)
        {
            try
            {
                _logger.LogInformation($"Attempting to update Book Id: {id}");
                var updatedBook = new Book
                {
                    Id = id,
                    BookName = book.BookName,
                    Category = book.Category,
                    Price = book.Price,
                    Author = book.Author
                };
                await _bookRepository.UpdateBook(id, updatedBook);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(UpdateBook)}: {ex.Message}");
                throw;
            }
        }
    }
}
