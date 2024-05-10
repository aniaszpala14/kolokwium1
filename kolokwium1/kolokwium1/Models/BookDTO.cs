namespace kolokwium1.Models;

public class BookDTO
{
    public int PK { get; set; }
    public string Title { get; set; }
    public List<GenreDTO> Genres { get; set; }
}

public class GenreDTO
{
    public string Name { get; set; }
}

public class AddBookWithGenresDTO
{
    public string Title { get; set; }
    public IEnumerable<GenreToAddDTO> Genres { get; set; }=new List<GenreToAddDTO>();
}

public class GenreToAddDTO
{
    public int PK { get; set; } 
}