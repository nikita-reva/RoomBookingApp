using Microsoft.AspNetCore.Mvc;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;

namespace RoomBookingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomBookingController : ControllerBase
    {
        private IRoomBookingRequestProcessor _roomBookingProcessor;

        public RoomBookingController(IRoomBookingRequestProcessor roomBookingProcessor)
        {
            _roomBookingProcessor = roomBookingProcessor;
        }

        [HttpPost(Name = "BookRoom")]
        public async Task<IActionResult> BookRoom(RoomBookingRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = _roomBookingProcessor.BookRoom(request);
                if (result.Flag == BookingResultFlag.Success)
                {
                    return Ok(result);
                }

                ModelState.AddModelError(nameof(RoomBookingRequest.Date), "No rooms available for given date");
            }

            return BadRequest(ModelState);
        }
    }
}
