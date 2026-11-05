using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Data.Sqlite;
using SecureBooksApi.Models;

namespace SecureBooksApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly SqliteConnection _connection;
        private readonly IAntiforgery _antiforgery;

        public BooksController(
            SqliteConnection connection,
            IAntiforgery antiforgery)
        {
            _connection = connection;
            _antiforgery = antiforgery;
        }

        [HttpGet("csrf-token")]
        public IActionResult GetCsrfToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            return Ok(new
            {
                token = tokens.RequestToken
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest("Invalid or missing anti-forgery token.");
            }

            using var command = _connection.CreateCommand();

            command.CommandText =
                "INSERT INTO Books (Title, Author) VALUES ($title, $author);";

            command.Parameters.AddWithValue("$title", book.Title);
            command.Parameters.AddWithValue("$author", book.Author);

            await command.ExecuteNonQueryAsync();

            return Ok(new
            {
                message = "Book added successfully!",
                title = book.Title,
                author = book.Author
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = new List<object>();

            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Author FROM Books;";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(new
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2)
                });
            }

            return Ok(books);
        }
    }
}