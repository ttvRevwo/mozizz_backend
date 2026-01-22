namespace MozizzAPI.DTOS
{
    public class MovieDto
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? Rating { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreateDate { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
