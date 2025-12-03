using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int ReservationId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string? TransactionReference { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;
}
