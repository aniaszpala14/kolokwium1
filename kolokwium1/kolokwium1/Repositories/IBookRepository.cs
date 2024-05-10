using kolokwium1.Models;

namespace kolokwium1.Repositories;

public interface IBookRepository
{

    Task<BookDTO> GetBookGenres(int id);
    Task AddBookWithGenres(AddBookWithGenresDTO bookWGenres);
    Task<bool> CheckBook(int id);
}