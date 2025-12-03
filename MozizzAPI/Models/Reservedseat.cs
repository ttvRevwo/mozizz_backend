using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Reservedseat
{
    public int ReservedSeatId { get; set; }

    public int ReservationId { get; set; }

    public int SeatId { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
