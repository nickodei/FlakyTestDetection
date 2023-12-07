namespace WebAPI;

using Microsoft.EntityFrameworkCore;

public class FlakyTestDetectionDbContext: DbContext
{
    public FlakyTestDetectionDbContext (DbContextOptions<FlakyTestDetectionDbContext> options) : base(options)
    {
    }
    
}