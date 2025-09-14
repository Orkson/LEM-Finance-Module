using Application.Devices.Commands;
using Application.Devices.Queries;
using Application.Documents;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Web.Controllers
{
    [Route("api/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDevice([FromForm] string addDeviceDto,
            [FromForm(Name = "deviceDocuments")] IFormFileCollection deviceDocuments,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(addDeviceDto))
            {
                return BadRequest("AddDeviceDto is required.");
            }

            try
            {
                var deviceDto = JsonSerializer.Deserialize<AddDeviceDto>(addDeviceDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (deviceDto == null)
                {
                    return BadRequest("Failed to deserialize AddDeviceDto.");
                }

                var command = new CreateDeviceCommand(deviceDto);
                var deviceIdentificationNumber = await _mediator.Send(command, cancellationToken);

                if (deviceDocuments != null && deviceDocuments.Any())
                {
                    var addDocumentsCommand = new AddDocumentsCommand((ICollection<IFormFile>)deviceDocuments, null, deviceIdentificationNumber.DeviceId);
                    await _mediator.Send(addDocumentsCommand, cancellationToken);
                }

                return Ok(deviceIdentificationNumber);
            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid AddDeviceDto format: {ex.Message}");
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDevices([FromQuery] PagedAndSortedDevicesListQueryDto pagedAndSortedDevicesQuery, CancellationToken cancellationToken)
        {
            var query = new GetAllDevicesQuery(pagedAndSortedDevicesQuery);
            var pagedAndSortedDevicesList = await _mediator.Send(query, cancellationToken);
            
            
                var x = Ok(pagedAndSortedDevicesList);
            return x;
        }

        [HttpGet("{deviceId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDeviceDetails([FromRoute] int deviceId, CancellationToken cancellationToken)
        {
            var query = new GetDeviceByIdQuery(deviceId);

            var deviceDetails = await _mediator.Send(query, cancellationToken);
            return Ok(deviceDetails);
        }

        [HttpPut("{deviceId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditDevice([FromRoute] int deviceId, [FromBody] EditDeviceRequestDto editDeviceDto, CancellationToken cancellationToken)
        {
            var command = new EditDeviceCommand(deviceId, editDeviceDto.OldDevice, editDeviceDto.NewDevice, editDeviceDto.ModelCooperationsToBeRemoved, cancellationToken);

            var editionResult = await _mediator.Send(command, cancellationToken);
            return Ok(editionResult);
        }

        [HttpDelete("{deviceId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveDevice([FromRoute] int deviceId, CancellationToken cancellationToken)
        {
            var command = new RemoveDeviceCommand(deviceId, cancellationToken);

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
