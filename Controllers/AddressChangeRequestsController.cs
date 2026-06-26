using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.DTOs;
using LogisticsManagementSystem.Enums;
using LogisticsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LogisticsManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressChangeRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressChangeRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(CreateAddressChangeRequestDto request)
        {
            var shipment = await _context.Shipments.FindAsync(request.ShipmentId);

            if (shipment == null)
                return NotFound("Shipment not found.");

            if (shipment.Status == ShipmentStatus.Delivered ||
                shipment.Status == ShipmentStatus.Returned ||
                shipment.Status == ShipmentStatus.Cancelled)
                return BadRequest("Address cannot be changed for completed or cancelled shipments.");

            var addressRequest = new AddressChangeRequest
            {
                ShipmentId = shipment.Id,
                CurrentDeliveryAddress = shipment.DeliveryAddress,
                RequestedDeliveryAddress = request.RequestedDeliveryAddress,
                Reason = request.Reason,
                Status = AddressChangeRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _context.AddressChangeRequests.AddAsync(addressRequest);
            await _context.SaveChangesAsync();

            return Ok(addressRequest);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _context.AddressChangeRequests
                .Where(r => r.Status == AddressChangeRequestStatus.Pending)
                .ToListAsync();

            return Ok(requests);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id, ReviewAddressChangeRequestDto request)
        {
            var addressRequest = await _context.AddressChangeRequests.FindAsync(id);

            if (addressRequest == null)
                return NotFound("Address change request not found.");

            if (addressRequest.Status != AddressChangeRequestStatus.Pending)
                return BadRequest("Only pending requests can be approved.");

            var shipment = await _context.Shipments.FindAsync(addressRequest.ShipmentId);

            if (shipment == null)
                return NotFound("Shipment not found.");

            shipment.DeliveryAddress = addressRequest.RequestedDeliveryAddress;

            addressRequest.Status = AddressChangeRequestStatus.Approved;
            addressRequest.ReviewedAt = DateTime.UtcNow;
            addressRequest.ReviewNotes = request.ReviewNotes;

            var history = new ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                Status = shipment.Status,
                Notes = $"Address change approved. New address: {shipment.DeliveryAddress}",
                ChangedAt = DateTime.UtcNow
            };

            await _context.ShipmentStatusHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id, ReviewAddressChangeRequestDto request)
        {
            var addressRequest = await _context.AddressChangeRequests.FindAsync(id);

            if (addressRequest == null)
                return NotFound("Address change request not found.");

            if (addressRequest.Status != AddressChangeRequestStatus.Pending)
                return BadRequest("Only pending requests can be rejected.");

            addressRequest.Status = AddressChangeRequestStatus.Rejected;
            addressRequest.ReviewedAt = DateTime.UtcNow;
            addressRequest.ReviewNotes = request.ReviewNotes;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}