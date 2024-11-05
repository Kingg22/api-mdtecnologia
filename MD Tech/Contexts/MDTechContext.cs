using Microsoft.EntityFrameworkCore;

namespace MD_Tech.Contexts
{
    public class MDTechContext : DbContext
    {
        public MDTechContext(DbContextOptions<MDTechContext> options) : base(options) { }
    }
}
