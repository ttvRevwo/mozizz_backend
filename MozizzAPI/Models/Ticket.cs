using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int ReservationId { get; set; }

    public string TicketCode { get; set; } = null!;

    public DateTime IssuedDate { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;
}
