using Bookstore.Books.Common.Models;
using Bookstore.Books.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Books.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService=bookService;
            _logger=logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> Get()
        {
            try
            {
                var books = await _bookService.GetBooks();

                return new OkObjectResult(books);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            try
            {
                var book = await _bookService.GetBookById(id);

                if (book is null)
                {
                    return NotFound();
                }

                return new OkObjectResult(book);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookRequestObject newBook)
        {
            try
            {
                await _bookService.CreateBook(newBook);

                return new OkObjectResult(newBook);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, BookRequestObject updatedBook)
        {
            try
            {
                var bookToUpdate = await _bookService.GetBookById(id);

                if (bookToUpdate is null)
                {
                    return NotFound();
                }

                await _bookService.UpdateBook(bookToUpdate.Id, updatedBook);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var bookToDelete = await _bookService.GetBookById(id);

                if (bookToDelete is null)
                {
                    return NotFound();
                }

                await _bookService.DeleteBook(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
