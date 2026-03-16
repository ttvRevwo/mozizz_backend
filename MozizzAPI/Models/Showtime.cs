using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Showtime
{
    public int ShowtimeId { get; set; }

    public int MovieId { get; set; }

    public int HallId { get; set; }

    public DateTime ShowDate { get; set; }

    public TimeSpan ShowTime1 { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Hall? Hall { get; set; }

    public virtual Movie? Movie { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}