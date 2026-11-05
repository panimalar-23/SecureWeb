using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Create and maintain an in-memory SQLite database.
var connection = new SqliteConnection("Data Source=BooksDb;Mode=Memory;Cache=Shared");
connection.Open();

var command = connection.CreateCommand();
command.CommandText = @"
    CREATE TABLE Books (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Title TEXT NOT NULL,
        Author TEXT NOT NULL
    );";
command.ExecuteNonQuery();

builder.Services.AddSingleton(connection);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();