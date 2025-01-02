using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Domain;
using RoomBookingApp.Domain.BaseModels;

namespace RoomBookingApp.Core.Processors
{
    public class RoomBookingRequestProcessor : IRoomBookingRequestProcessor
    {
        private readonly IRoomBookingService _roomBookingService;

        public RoomBookingRequestProcessor(IRoomBookingService roomBookingService)
        {
            _roomBookingService = roomBookingService;
        }

        public BookRoomResult BookRoom(RoomBookingRequest? request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var availableRooms = _roomBookingService.GetAvailableRooms(request.Date);
            var result = CreateRoomBookingObject<BookRoomResult>(request);

            if (availableRooms.Any())
            {
                var room = availableRooms.First();
                var roomBooking = CreateRoomBookingObject<RoomBooking>(request);
                roomBooking.RoomId = room.Id;
                _roomBookingService.Save(roomBooking);
                result.RoomBookingId = roomBooking.Id;
                result.Flag = BookingResultFlag.Success;
            }
            else
            {
                result.Flag = BookingResultFlag.Failure;
            }

            return result;
        }

        private TRoomBooking CreateRoomBookingObject<TRoomBooking>(RoomBookingRequest request)
            where TRoomBooking : RoomBookingBase, new()
        {
            return new TRoomBooking
            {
                FullName = request.FullName,
                Email = request.Email,
                Date = request.Date,
            };
        }
    }
}