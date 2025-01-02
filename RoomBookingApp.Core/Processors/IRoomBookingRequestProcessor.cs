using RoomBookingApp.Core.Models;

namespace RoomBookingApp.Core.Processors
{
    public interface IRoomBookingRequestProcessor
    {
        BookRoomResult BookRoom(RoomBookingRequest? request);
    }
}