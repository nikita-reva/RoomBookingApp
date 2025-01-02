using Microsoft.Extensions.Logging;
using Moq;
using RoomBookingApp.Api.Controllers;
using Shouldly;

namespace RoomBookingApp.Api.Tests
{
    public class WeatherForecastTest
    {
        [Fact]
        public void SHould_Return_Forecast_Results()
        {
            var loggerMock = new Mock<ILogger<WeatherForecastController>>();
            var controller = new WeatherForecastController(loggerMock.Object);

            var result = controller.Get();

            result.Count().ShouldBeGreaterThan(1);
            result.ShouldNotBeNull();
        }
    }
}