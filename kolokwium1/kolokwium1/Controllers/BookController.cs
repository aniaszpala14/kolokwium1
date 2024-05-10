using Microsoft.AspNetCore.Mvc;

using kolokwium1.Models;
using kolokwium1.Repositories;

namespace kolokwium1.Controllers;

[Route("api/books")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetBookGenres(int id)
    {
        if (!await _bookRepository.CheckBook(id))
            return NotFound("Nie ma takiej ksiazki");
        var result = await _bookRepository.GetBookGenres(id);
        return Ok(result);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddBookWithAuthors([FromBody]AddBookWithGenresDTO bookWGenres)
    {
        await _bookRepository.AddBookWithGenres(bookWGenres);
        return Created(Request.Path.Value ?? "api/books", bookWGenres);
    }

}