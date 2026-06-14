using LogisticsManagementSystem.Models;
using LogisticsManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentRepository _shipmentRepository;

        public ShipmentsController(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetAllShipments()
        {
            var shipments = await _shipmentRepository.GetAllShipmentsAsync();
            return Ok(shipments);
        }

        [HttpGet("{id}", Name = "GetShipmentById")]
        public async Task<ActionResult<Shipment>> GetShipmentById(int id)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(id);

            if (shipment == null)
            {
                return NotFound();
            }

            return Ok(shipment);
        }

        [HttpGet("track/{trackingNumber}")]
        public async Task<ActionResult<Shipment>> TrackShipment(string trackingNumber)
        {
            var shipment = await _shipmentRepository
                .GetShipmentByTrackingNumberAsync(trackingNumber);

            if (shipment == null)
            {
                return NotFound();
            }

            return Ok(shipment);
        }

        [HttpPost]
        public async Task<ActionResult<Shipment>> CreateShipment(Shipment shipment)
        {
            await _shipmentRepository.AddShipmentAsync(shipment);

            return CreatedAtRoute(
                "GetShipmentById",
                new { id = shipment.Id },
                shipment
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, Shipment shipment)
        {
            if (id != shipment.Id)
            {
                return BadRequest("Shipment ID mismatch.");
            }

            await _shipmentRepository.UpdateShipmentAsync(shipment);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            await _shipmentRepository.DeleteShipmentAsync(id);

            return NoContent();
        }
    }
}