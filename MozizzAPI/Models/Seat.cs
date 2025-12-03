using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int HallId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool? IsVip { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual ICollection<Reservedseat> Reservedseats { get; set; } = new List<Reservedseat>();
}
