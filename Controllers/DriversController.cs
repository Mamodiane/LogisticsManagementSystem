using LogisticsManagementSystem.Models;
using LogisticsManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using LogisticsManagementSystem.DTOs;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
        {
            var drivers = await _driverRepository.GetAllDriversAsync();
            return Ok(drivers);
        }

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

        [HttpPost]
        public async Task<ActionResult<Driver>> CreateDriver(Driver driver)
        {
            await _driverRepository.AddDriverAsync(driver);
            return Ok(driver);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            await _driverRepository.DeleteDriverAsync(id);
            return NoContent();
        }

        // Mark shipment as collected
        [HttpPut("{driverId}/shipments/{shipmentId}/collect")]
        public async Task<IActionResult> MarkShipmentAsCollected(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsCollectedAsync(driverId, shipmentId);
            return NoContent();
        }
        // Mark shipment as in transit
        [HttpPut("{driverId}/shipments/{shipmentId}/in-transit")]
        public async Task<IActionResult> MarkShipmentAsInTransit(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsInTransitAsync(driverId, shipmentId);
            return NoContent();
        }
        // Mark shipment as delivered
        [HttpPut("{driverId}/shipments/{shipmentId}/deliver")]
        public async Task<IActionResult> MarkShipmentAsDelivered(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsDeliveredAsync(driverId, shipmentId);
            return NoContent();
        }

        //
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
        [HttpPut("{driverId}/shipments/{shipmentId}/return")]
        public async Task<IActionResult> MarkShipmentAsReturned(int driverId, int shipmentId)
        {
            await _driverRepository.MarkShipmentAsReturnedAsync(driverId, shipmentId);
            return NoContent();
        }

        // Add delivery proof
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
    }
}