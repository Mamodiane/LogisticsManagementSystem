using LogisticsManagementSystem.Models;
using LogisticsManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using LogisticsManagementSystem.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace LogisticsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;

        public DriversController(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
        {
            var drivers = await _driverRepository.GetAllDriversAsync();
            return Ok(drivers);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriverById(int id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        //get assigned shipments for a driver
        [HttpGet("{driverId}/shipments")]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetAssignedShipments(int driverId)
        {
            var shipments = await _driverRepository.GetAssignedShipmentsAsync(driverId);

            return Ok(shipments);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
        {
            await _driverRepository.AddDriverAsync(driver);
            return Ok(driver);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, Driver driver)
        {
            if (id != driver.Id)
            {
                return BadRequest("Driver ID mismatch.");
            }

            await _driverRepository.UpdateDriverAsync(driver);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            await _driverRepository.DeleteDriverAsync(id);
            return NoContent();
        }

        // Mark shipment as collected
        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("{driverId}/shipments/{shipmentId}/collect")]
        public async Task<IActionResult> MarkShipmentAsCollected(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsCollectedAsync(driverId, shipmentId);
            return NoContent();
        }
        // Mark shipment as in transit
        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("{driverId}/shipments/{shipmentId}/in-transit")]
        public async Task<IActionResult> MarkShipmentAsInTransit(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsInTransitAsync(driverId, shipmentId);
            return NoContent();
        }
        // Mark shipment as delivered
        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("{driverId}/shipments/{shipmentId}/deliver")]
        public async Task<IActionResult> MarkShipmentAsDelivered(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsDeliveredAsync(driverId, shipmentId);
            return NoContent();
        }

        //
        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("{driverId}/shipments/{shipmentId}/fail")]
        public async Task<IActionResult> MarkShipmentAsFailedDelivery(
        int driverId,
        int shipmentId,
        FailedDeliveryRequest request)
         {
                if (string.IsNullOrWhiteSpace(request.Reason))
                {
                    return BadRequest("Failure reason is required.");
                }

                await _driverRepository.MarkShipmentAsFailedDeliveryAsync(
                    driverId,
                    shipmentId,
                    request.Reason
                );

                return NoContent();
         }

        //
        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("{driverId}/shipments/{shipmentId}/return")]
        public async Task<IActionResult> MarkShipmentAsReturned(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsReturnedAsync(driverId, shipmentId);
            return NoContent();
        }

        // Add delivery proof
        [Authorize(Roles = "Admin,Driver")]
        [HttpPost("{driverId}/shipments/{shipmentId}/delivery-proof")]
        public async Task<IActionResult> AddDeliveryProof(
    int driverId,
    int shipmentId,
    CreateDeliveryProofDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ReceiverName))
            {
                return BadRequest("Receiver name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.SignatureName))
            {
                return BadRequest("Signature name is required.");
            }

            await _driverRepository.AddDeliveryProofAsync(driverId, shipmentId, request);

            return NoContent();
        }
        // Get shipments assigned to the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpGet("my-shipments")]
        public async Task<IActionResult> GetMyShipments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var applicationUserId = int.Parse(userIdClaim);

            var driver = await _driverRepository
                .GetDriverByApplicationUserIdAsync(applicationUserId);

            if (driver == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            var shipments = await _driverRepository
                .GetAssignedShipmentsAsync(driver.Id);

            return Ok(shipments);
        }

        // Link driver to user account
        [Authorize(Roles = "Admin")]
        [HttpPut("{driverId}/link-user")]
        public async Task<IActionResult> LinkDriverToUser(
    int driverId,
    LinkDriverUserDto request)
        {
            await _driverRepository.LinkDriverToUserAsync(
                driverId,
                request.ApplicationUserId
            );

            return NoContent();
        }
        // Get driver profile for the currently authenticated user
        private async Task<int?> GetCurrentDriverIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return null;
            }

            var applicationUserId = int.Parse(userIdClaim);

            var driver = await _driverRepository
                .GetDriverByApplicationUserIdAsync(applicationUserId);

            return driver?.Id;
        }
        // Mark shipment as collected for the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpPut("my-shipments/{shipmentId}/collect")]
        public async Task<IActionResult> MarkMyShipmentAsCollected(int shipmentId)
        {
            var driverId = await GetCurrentDriverIdAsync();

            if (driverId == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            await _driverRepository.MarkShipmentAsCollectedAsync(driverId.Value, shipmentId);

            return NoContent();
        }
        // Mark shipment as in transit for the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpPut("my-shipments/{shipmentId}/in-transit")]
        public async Task<IActionResult> MarkMyShipmentAsInTransit(int shipmentId)
        {
            var driverId = await GetCurrentDriverIdAsync();

            if (driverId == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            await _driverRepository.MarkShipmentAsInTransitAsync(driverId.Value, shipmentId);

            return NoContent();
        }
        // Mark shipment as delivered for the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpPut("my-shipments/{shipmentId}/deliver")]
        public async Task<IActionResult> MarkMyShipmentAsDelivered(int shipmentId)
        {
            var driverId = await GetCurrentDriverIdAsync();

            if (driverId == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            await _driverRepository.MarkShipmentAsDeliveredAsync(driverId.Value, shipmentId);

            return NoContent();
        }
        // Mark shipment as failed delivery for the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpPut("my-shipments/{shipmentId}/fail")]
        public async Task<IActionResult> MarkMyShipmentAsFailedDelivery(
    int shipmentId,
    FailedDeliveryRequest request)
        {
            var driverId = await GetCurrentDriverIdAsync();

            if (driverId == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest("Failure reason is required.");
            }

            await _driverRepository.MarkShipmentAsFailedDeliveryAsync(
                driverId.Value,
                shipmentId,
                request.Reason
            );

            return NoContent();
        }

        // Mark shipment as returned for the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpPut("my-shipments/{shipmentId}/return")]
        public async Task<IActionResult> MarkMyShipmentAsReturned(int shipmentId)
        {
            var driverId = await GetCurrentDriverIdAsync();

            if (driverId == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            await _driverRepository.MarkShipmentAsReturnedAsync(
                driverId.Value,
                shipmentId
            );

            return NoContent();
        }
        // Add delivery proof for the currently authenticated driver
        [Authorize(Roles = "Driver")]
        [HttpPost("my-shipments/{shipmentId}/delivery-proof")]
        public async Task<IActionResult> AddMyDeliveryProof(
    int shipmentId,
    CreateDeliveryProofDto request)
        {
            var driverId = await GetCurrentDriverIdAsync();

            if (driverId == null)
            {
                return NotFound("No driver profile linked to this user account.");
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverName))
            {
                return BadRequest("Receiver name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.SignatureName))
            {
                return BadRequest("Signature name is required.");
            }

            await _driverRepository.AddDeliveryProofAsync(
                driverId.Value,
                shipmentId,
                request
            );

            return NoContent();
        }
    }
}