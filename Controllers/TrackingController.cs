using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrackingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{trackingNumber}")]
        public async Task<ActionResult<ShipmentTrackingDto>> TrackShipment(string trackingNumber)
        {
            var shipment = await _context.Shipments
                .Include(s => s.StatusHistory)
                .FirstOrDefaultAsync(s =>
                    s.TrackingNumber.ToUpper() == trackingNumber.Trim().ToUpper()
                );

            if (shipment == null)
            {
                return NotFound("Shipment not found.");
            }

            var result = new ShipmentTrackingDto
            {
                TrackingNumber = shipment.TrackingNumber,
                ReceiverName = shipment.ReceiverName,
                PickupAddress = shipment.PickupAddress,
                DeliveryAddress = shipment.DeliveryAddress,
                Status = shipment.Status.ToString(),
                History = shipment.StatusHistory
                    .OrderBy(h => h.ChangedAt)
                    .Select(h => new TrackingHistoryDto
                    {
                        Status = h.Status.ToString(),
                        Notes = h.Notes,
                        ChangedAt = h.ChangedAt
                    })
                    .ToList()
            };

            return Ok(result);
        }
    }
}