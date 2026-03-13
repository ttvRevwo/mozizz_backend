namespace MozizzAPI.Models;

public partial class BuffetItem
{
    public int ItemId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Price { get; set; }
    public string Category { get; set; } = "snack";
    public string? Img { get; set; }
    public bool IsAvailable { get; set; } = true;
}