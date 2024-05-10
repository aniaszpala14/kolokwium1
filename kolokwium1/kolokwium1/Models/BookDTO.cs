namespace kolokwium1.Models;

public class BookDTO
{
    public int PK { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<GenreDTO> Genres { get; set; }
}

public class GenreDTO
{
    public string Name { get; set; }
}

public class AddBookWithGenresDTO
{
    public string Title { get; set; }
  //  public IEnumerable<GenreToAddDTO> Genres { get; set; }=new List<GenreToAddDTO>();
    public IEnumerable<int> Genres { get; set; }=new List<int>();
}

public class GenreToAddDTO
{
    public int PK { get; set; } 
}