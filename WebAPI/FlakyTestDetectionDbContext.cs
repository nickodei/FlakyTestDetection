namespace WebAPI;

using Microsoft.EntityFrameworkCore;

public class FlakyTestDetectionDbContext(DbContextOptions<FlakyTestDetectionDbContext> options) : DbContext(options);