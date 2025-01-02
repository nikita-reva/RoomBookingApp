using RoomBookingApp.Core.Enums;
using RoomBookingApp.Domain.BaseModels;

namespace RoomBookingApp.Core.Models
{
    public class BookRoomResult : RoomBookingBase
    {
        public BookingResultFlag Flag { get; set; }

        public int? RoomBookingId { get; set; }
    }

    public record RecordRoomBookingResult(string FullName, string Email, DateTime Date);

    public static class RoomBookingResultExtensions
    {
        public static RecordRoomBookingResult ToRecord(this BookRoomResult input)
        {
            return new RecordRoomBookingResult(input.FullName, input.Email, input.Date);
        }
    }
}