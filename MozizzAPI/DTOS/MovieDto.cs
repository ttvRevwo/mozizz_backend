namespace MozizzAPI.DTOS
{
    public class MovieDto
    {
        public int MovieId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Genre { get; set; }
        public required string Rating { get; set; }
        public required int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreateDate { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
