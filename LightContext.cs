using Microsoft.EntityFrameworkCore;
 
namespace lightsmeai.Models
{
    public class LightContext : DbContext
    {
        public LightContext(DbContextOptions<LightContext> options) : base(options)
        {
        }
 
        public DbSet<Light> Lights { get; set; }
    }
}