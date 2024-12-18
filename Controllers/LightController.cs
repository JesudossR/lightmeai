using lightsmeai.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lightsmeai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LightController : ControllerBase
    {
        // Initialize the context within the constructor
        private readonly LightContext _context;

        public LightController()
        {
            var options = new DbContextOptionsBuilder<LightContext>()
                .UseInMemoryDatabase("LightList")
                .Options;
            _context = new LightContext(options);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Light>>> GetLights()
        {
            Console.WriteLine("LightController GetLights -> ");
            return await _context.Lights.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Light>> GetLight(int id)
        {
            Console.WriteLine("LightController GetLight -> " + id);
            var light = await _context.Lights.FindAsync(id);

            if (light == null)
            {
                return NotFound();
            }

            return light;
        }

        [HttpPost]
        public async Task<ActionResult<Light>> AddLight(Light light)
        {
            Console.WriteLine("LightController PostLight -> " + light);
            _context.Lights.Add(light);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLight), new { id = light.Id }, light);
        }

        [HttpPost("multiple")]
        public async Task<ActionResult<IEnumerable<Light>>> AddMultipleLights(IEnumerable<Light> lights)
        {
            _context.Lights.AddRange(lights);
            await _context.SaveChangesAsync();

            // Use the first light's ID for the URI in the response 
            return CreatedAtAction(nameof(GetLight), new { id = lights.First().Id }, lights);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLight(int id, Light light)
        {
            Console.WriteLine("LightController UpdateLight -> " + id, light);
            if (id != light.Id)
            {
                return BadRequest();
            }

            // Ensure light.Id is set to the provided id 
            // light.Id = id;

            // var updatedlight = new Light { 
            //     Id = id, 
            //     Name = light.Name, 
            //     Switched = light.Switched 
            //     };
            _context.Entry(light).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLight(int id)
        {
            Console.WriteLine("LightController DeleteLight -> " + id);
            var light = await _context.Lights.FindAsync(id);
            if (light == null)
            {
                return NotFound();
            }

            _context.Lights.Remove(light);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
    }
}
