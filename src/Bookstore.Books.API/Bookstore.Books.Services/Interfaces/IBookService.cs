using Bookstore.Books.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Books.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetBooks();
        Task<Book> GetBookById(string id);
        Task CreateBook(BookRequestObject book);
        Task UpdateBook(string id, BookRequestObject book);
        Task DeleteBook(string id);
    }
}
