using LogisticsManagementSystem.Models;
using LogisticsManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

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
    }
}