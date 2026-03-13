namespace MozizzAPI.DTOS
{
    public class BuffetItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Category { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
