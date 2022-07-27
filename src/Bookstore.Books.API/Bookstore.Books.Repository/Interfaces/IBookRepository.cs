using Bookstore.Books.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Books.Repository.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllBooks();
        Task<Book> GetBookById(string id);
        Task CreateBook(Book newBook);
        Task UpdateBook(string id, Book book);
        Task DeleteBook(string id);
    }
}
