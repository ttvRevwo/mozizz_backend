using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Genre { get; set; }

    public int Duration { get; set; }

    public string? Rating { get; set; }

    public string? Description { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
