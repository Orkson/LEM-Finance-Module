using Microsoft.AspNetCore.Mvc;
using TestLEM.Models;
using TestLEM.Services;

namespace TestLEM.Controllers
{
    [Route("api/devices")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            this.deviceService = deviceService;
        }

        [HttpPost]
        public ActionResult<AddDeviceDto> AddDevice([FromBody] AddDeviceDto deviceDto)
        { 
            deviceService.AddDeviceToDatabase(deviceDto);
            return Ok(deviceDto);
        }
    }
}
