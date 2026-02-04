namespace MozizzAPI.DTOS
{
    public class BookingDto
    {
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } 
    }
}