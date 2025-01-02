using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Domain;
using Shouldly;

namespace RoomBookingApp.Core.Tests
{
    public class RoomBookingRequestProcessorTest
    {
        private RoomBookingRequestProcessor _processor;
        private RoomBookingRequest _request;
        private List<Room> _availableRooms;
        private Mock<IRoomBookingService> _roomBookingServiceMock;

        public RoomBookingRequestProcessorTest()
        {
            _request = new RoomBookingRequest
            {
                FullName = "Test name",
                Email = "test@request.com",
                Date = new DateTime(2024, 12, 28),
            };

            _availableRooms = new List<Room>()
            {
                new Room() { Id = 1 }
            };

            _roomBookingServiceMock = new Mock<IRoomBookingService>();
            _roomBookingServiceMock.Setup(q => q.GetAvailableRooms(_request.Date)).Returns(_availableRooms);
            _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
        }

        [Fact]
        public void Should_Return_Room_Booking_Request()
        {
            // Arrange

            // Act
            BookRoomResult result = _processor.BookRoom(_request);
            RecordRoomBookingResult recordResult = result.ToRecord();

            // Assert
            // xUnit
            Assert.NotNull(recordResult);
            Assert.Equal(recordResult.FullName, _request.FullName);
            Assert.Equal(recordResult.Email, _request.Email);
            Assert.Equal(recordResult.Date, _request.Date);

            // Shouldly
            result.ShouldNotBeNull();
            result.FullName.ShouldBe(_request.FullName);
            result.Email.ShouldBe(_request.Email);
            result.Date.ShouldBe(_request.Date);
        }

        [Fact]
        public void Should_Throw_Exception_For_Null_Request()
        {
            // Arrange

            // Act

            // Assert
            var exception = Should.Throw<ArgumentNullException>(() => _processor.BookRoom(null));
            exception.ParamName.ShouldBe("request");
        }

        [Fact]
        public void Should_Save_Room_Booking_Request()
        {
            // Arrange
            RoomBooking? savedBooking = null;

            _roomBookingServiceMock.Setup((q) => q.Save(It.IsAny<RoomBooking>())).Callback<RoomBooking>(booking =>
            {
                savedBooking = booking;
            });

            // Act
            _processor.BookRoom(_request);

            // Assert
            _roomBookingServiceMock.Verify((q) => q.Save(It.IsAny<RoomBooking>()), Times.Once);
            savedBooking.ShouldNotBeNull();
            savedBooking.FullName.ShouldBe(_request.FullName);
            savedBooking.Email.ShouldBe(_request.Email);
            savedBooking.Date.ShouldBe(_request.Date);
            savedBooking.RoomId.ShouldBe(_availableRooms.First().Id);
        }

        [Fact]
        public void Should_Not_Save_Room_Booking_Request_If_None_Available()
        {
            // Arrange
            _availableRooms.Clear();

            // Act
            _processor.BookRoom(_request);

            // Assert
            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(BookingResultFlag.Failure, false)]
        [InlineData(BookingResultFlag.Success, true)]
        public void Should_Return_Success_Or_Failure_Flag_In_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
        {
            // Arrange
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }

            // Act
            var result = _processor.BookRoom(_request);

            // Assert
            bookingSuccessFlag.ShouldBe(result.Flag);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(null, false)]
        public void Should_Return_RoomBookingId_In_Result(int? roomBookingId, bool isAvailable)
        {
            // Arrange
            if (!isAvailable)
            {
                _availableRooms.Clear();
            }
            else
            {
                _roomBookingServiceMock.Setup((q) => q.Save(It.IsAny<RoomBooking>())).Callback<RoomBooking>(booking =>
                {
                    booking.Id = roomBookingId != null ? roomBookingId.Value : null;
                });
            }

            // Act
            var result = _processor.BookRoom(_request);

            // Assert
            result.RoomBookingId.ShouldBe(roomBookingId);
        }
    }
}