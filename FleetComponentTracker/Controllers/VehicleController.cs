using FleetComponentTracker.Models;
using FleetComponentTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetComponentTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        //Dependency Injection
        private readonly VehicleDataService _vehicleService;

        public VehicleController(VehicleDataService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        //Controller call to get all component data
        // GET: api/vehicle
        [HttpGet]
        public async Task<IActionResult> GetAllComponents()
        {
            var components = await _vehicleService.GetAllComponentsAsync();
            return Ok(components);
        }

        //Controller call to get component via id
        // GET: api/vehicle/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetComponent(int id)
        {
            var component = await _vehicleService.GetComponentByIdAsync(id);
            if (component == null)
                return NotFound();

            return Ok(component);
        }

        //Controller call to add component to db (parameter takes component seralized by JSON, deseralizes it)
        // POST: api/vehicle
        [HttpPost]
        public async Task<IActionResult> AddComponent([FromBody] FleetComponentTracker.Models.Components newComponent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _vehicleService.AddComponentAsync(newComponent);
            return CreatedAtAction(nameof(GetComponent), new { id = newComponent.Id }, newComponent);
        }

        //Controller call to update component on db (parameter takes component seralized by JSON, deseralizes it)
        // PUT: api/vehicle/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateComponent(int id, [FromBody] FleetComponentTracker.Models.Components updatedComponent)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updatedComponent.Id)
                return BadRequest("ID mismatch.");

            var existing = await _vehicleService.GetComponentByIdAsync(id);
            if (existing == null)
                return NotFound();

            try
            {
                await _vehicleService.UpdateComponentAsync(updatedComponent);
                return NoContent();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { message = errorMessage });
            }
        }

        //Controller call to delete component on db
        // DELETE: api/vehicle/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var existing = await _vehicleService.GetComponentByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _vehicleService.DeleteComponentAsync(id);
            return NoContent();
        }
    }
}
