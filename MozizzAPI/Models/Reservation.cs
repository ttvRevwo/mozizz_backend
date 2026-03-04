using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MozizzAPI.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public int ShowtimeId { get; set; }

    public DateTime ReservationDate { get; set; }

    public string? Status { get; set; }

    [Column("is_reminder_sent")]
    public bool? IsReminderSent { get; set; } = false;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reservedseat> Reservedseats { get; set; } = new List<Reservedseat>();

    public virtual Showtime Showtime { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
