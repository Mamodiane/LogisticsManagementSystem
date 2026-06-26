using LogisticsManagementSystem.Data;
using LogisticsManagementSystem.DTOs;
using LogisticsManagementSystem.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace LogisticsManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var stats = new DashboardStatsDto
            {
                TotalShipments = await _context.Shipments.CountAsync(),
                Pending = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.Pending),
                Assigned = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.Assigned),
                Collected = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.Collected),
                InTransit = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.InTransit),
                Delivered = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.Delivered),
                FailedDelivery = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.FailedDelivery),
                Returned = await _context.Shipments.CountAsync(s => s.Status == ShipmentStatus.Returned),
                AvailableDrivers = await _context.Drivers.CountAsync(d => d.IsAvailable)
            };

            return Ok(stats);
        }
    }
}