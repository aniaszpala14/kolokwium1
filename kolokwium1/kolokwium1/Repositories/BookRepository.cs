
using kolokwium1.Models;
using Microsoft.Data.SqlClient;

namespace kolokwium1.Repositories;

public class BookRepository : IBookRepository
{
    public readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<BookDTO> GetBookGenres(int id)
    {
        var query =  @"SELECT Books.PK AS BOOKPK ,Title,Genres.PK,Name FROM BOOKS 
                JOIN BOOKS_GENRES ON Books_Genres.FK_book=Books.PK
                JOIN GENRES ON Genres.PK=Books_Genres.FK_genre
                WHERE Books.PK=@Id";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
    
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
    
        await connection.OpenAsync();
    
        var reader = await command.ExecuteReaderAsync();
        
        var BookPKOrdinal = reader.GetOrdinal("BOOKPK");
        var BookTitleOrdinal = reader.GetOrdinal("Title");
    
        var GenresOrdinal = reader.GetOrdinal("Name");
        
        BookDTO bookDto = null;
        
        while (await reader.ReadAsync())
        {
            if (bookDto is not null)
            {
                bookDto.Genres.Add(new GenreDTO()
                {
                    Name = reader.GetString(GenresOrdinal),
                });
            }
            else
            {
                bookDto = new BookDTO()
                {
                    PK = reader.GetInt32(BookPKOrdinal),
                    Title = reader.GetString(BookTitleOrdinal),
                    Genres = new List<GenreDTO>()
                    {
                        new GenreDTO()
                        {
                            Name = reader.GetString(GenresOrdinal)
                        }
                    }
                };
        
            }
        }
        if (bookDto is null) throw new Exception();
        return bookDto;
    
    }
        public async Task AddBookWithGenres(AddBookWithGenresDTO bookWGenres)
    {
         var insert = @"INSERT INTO Books (Title) VALUES (@Title); SELECT SCOPE_IDENTITY();";
        // var insert = "INSERT INTO Books (PK, Title) VALUES ((SELECT ISNULL(MAX(PK), 0) + 1 FROM Books), @Title)"; 

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = insert;
        command.Parameters.AddWithValue("@Title", bookWGenres.Title);
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            var bookId = await command.ExecuteScalarAsync();
            
            command.CommandText = "SELECT ISNULL(MAX(PK), 0) FROM BOOKS";
            var book2Id = await command.ExecuteScalarAsync();
            
            foreach (var genre in bookWGenres.Genres)
            {
                command.Parameters.Clear();
              // command.CommandText="INSERT INTO Genres (Name) VALUES(@Name);SELECT SCOPE_IDENTITY();";
                // command.CommandText="INSERT INTO Genres (PK,Name) VALUES((SELECT ISNULL(MAX(PK), 0) + 1 FROM Genres),@Name);";

                 command.CommandText="SELECT PK FROM Genres WHERE PK=@PK";
                 command.Parameters.AddWithValue("@PK",genre.PK);
                 //await command.ExecuteNonQueryAsync();
                 
                // command.CommandText = "SELECT ISNULL(MAX(PK), 0) FROM Genres";
                 var genreId = await command.ExecuteScalarAsync();

             
                 command.CommandText =  "INSERT INTO books_genres (FK_book, FK_genre) VALUES (@BooksPK, @GenresPK);";;
                 command.Parameters.Clear();
                 command.Parameters.AddWithValue("@BooksPK", book2Id);
                 command.Parameters.AddWithValue("@GenresPK", genreId);
                 await command.ExecuteNonQueryAsync();
                
            }
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        
    }
    
    public async Task<bool> CheckBook(int id)
    {
        var query = "SELECT 1 FROM Books WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res is not null; 
    }
    public async Task<bool> CheckGenre(int id)
    {
        var query = "SELECT 1 FROM Genre WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res is not null; 
    }
    



}